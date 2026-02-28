using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(CastleReachCheckSystem))]
public partial struct EnemyMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        new EnemyMoveJob
        {
            DeltaTime = dt
        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct EnemyMoveJob : IJobEntity
    {
        public float DeltaTime;

        void Execute(
            ref LocalTransform transform,
            in MoveSpeedComponent speed,
            in MoveDirection direction,
            in EnemyTag tag)
        {
            transform.Position += direction.Value * speed.Value * DeltaTime;
        }
    }
}