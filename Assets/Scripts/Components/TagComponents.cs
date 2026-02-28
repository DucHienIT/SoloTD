using Unity.Entities;

/// <summary>Tag: marks entity as a Hero</summary>
public struct HeroTag : IComponentData { }

/// <summary>Tag: marks entity as an Enemy</summary>
public struct EnemyTag : IComponentData { }

/// <summary>Tag: marks entity as a Projectile</summary>
public struct ProjectileTag : IComponentData { }

/// <summary>Tag: marks entity as the Castle</summary>
public struct CastleTag : IComponentData { }

/// <summary>Tag: enemy reached castle line, pending damage apply</summary>
public struct ReachedCastleTag : IComponentData { }

/// <summary>Tag: entity pending destruction this frame</summary>
public struct DestroyTag : IComponentData { }
