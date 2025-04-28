# 플레이어 공격 기능
### Player Attack
**플레이어가 공격 대상을 판별하고 공격을 수행하는 클래스**

+prefab: Projectile

+angle: float

+sightRange: float

+gazeTrasform: Transform

+target: GameObject

+shootCoroutine: Coroutine

+shootDelay: YieldInstruction


<br>

+Update(): void

+Shoot(shootCount): IEnumerator

+DetectMonster(): void

+TraceMonster(): void

+GiveDamage(GameObject gameObject, int prefab.damage): void


### Projectile
**발사체별 공격속성을 담고있는 클래스**

+damage: int

+rigid: Rigidbody

+arrowPrefab: GameObject


<br>

+Start(): void

+OnCollisionEnter(collider collision)


### ArrowObjectPool
**화살 ObjectPool 관리하는 클래스**
(Player Attack, Projectile 완성 후 구현)
