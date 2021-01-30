using UnityEngine;

namespace ToExport.Scripts.Player.Combat
{
    public class ProjectileFactory
    {
        private readonly GameObject _bulletPrefab;
        private readonly int _damage;
        private readonly float _speed;

        public ProjectileFactory(GameObject bulletPrefab, int damage, float speed)
        {
            _bulletPrefab = bulletPrefab;
            _damage = damage;
            _speed = speed;
        }
        
        public Projectile Create(Vector3 barrelLocation, Vector3 shotDirection)
        {
            var projectile = Object.Instantiate(_bulletPrefab, barrelLocation, Quaternion.Euler(shotDirection.x, shotDirection.y, shotDirection.z)).GetComponent<Projectile>();
            projectile.SetParameters(_damage, _speed);
            return projectile;
        }
    }
}