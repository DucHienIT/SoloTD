using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(HeroAttackSystem))]
public partial struct HeroTargetingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HeroTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Collect all alive enemies with positions
        var enemyQuery = SystemAPI.QueryBuilder()
            .WithAll<EnemyTag, LocalTransform, Health>()
            .WithNone<DestroyTag>()
            .Build();

        var enemyEntities = enemyQuery.ToEntityArray(Allocator.TempJob);
        var enemyTransforms = enemyQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        // Find castle line Y for priority calculation
        float castleLineY = -4f;
        foreach (var castleLine in SystemAPI.Query<RefRO<CastleLine>>().WithAll<CastleTag>())
        {
            castleLineY = castleLine.ValueRO.YPosition;
            break;
        }

        new HeroTargetJob
        {
            EnemyEntities = enemyEntities,
            EnemyTransforms = enemyTransforms,
            CastleLineY = castleLineY
        }.ScheduleParallel();

        // Dependency will handle disposal timing,
        // but we need to manage the arrays
        state.Dependency.Complete();
        enemyEntities.Dispose();
        enemyTransforms.Dispose();
    }

    [BurstCompile]
    partial struct HeroTargetJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> EnemyEntities;
        [ReadOnly] public NativeArray<LocalTransform> EnemyTransforms;
        public float CastleLineY;

        void Execute(
            ref TargetEntity target,
            in LocalTransform heroTransform,
            in HeroAttack attack,
            in HeroTag tag)
        {
            Entity bestTarget = Entity.Null;
            float bestScore = float.MaxValue;

            for (int i = 0; i < EnemyEntities.Length; i++)
            {
                float3 enemyPos = EnemyTransforms[i].Position;
                float dist = math.distance(heroTransform.Position, enemyPos);

                if (dist > attack.Range) continue;

                // Priority: closest to castle line (lowest Y = closest),
                // then nearest distance to hero
                float distToCastle = enemyPos.y - CastleLineY;
                float score = distToCastle * 10f + dist;

                if (score < bestScore)
                {
                    bestScore = score;
                    bestTarget = EnemyEntities[i];
                }
            }

            target.Value = bestTarget;
        }
    }
}