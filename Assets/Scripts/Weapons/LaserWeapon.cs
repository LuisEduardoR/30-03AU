using UnityEngine;

// Esse código serve como base para armas laser.
[AddComponentMenu("Scripts/Weapons/Laser Weapon")]
public class LaserWeapon : WeaponBase {

    // Máscara usada para ignorar certos objetos.
    public LayerMask raycastMask;
    
    public int damage = 10;
    // Tempo que a arma deixa o alvo paralisado, 0 = sem paralisia.
    public float paralyse = 0;

    // Direção para onde o laser vai (Deve ser -1 ou 1, outros valores podem causar problemas).
    public int direction = 1;

    private bool showingLaser = false;
    public LineRenderer _laserGraphics;

    public override void Update() {

        if (!showingLaser && _laserGraphics.enabled) {
            _laserGraphics.gameObject.SetActive(false);
        }
        showingLaser = false;

        base.Update();

    }

    public override void Fire() {

        if (currentAmmo > 0 || infinite_ammo) {

            showingLaser = true;
            _laserGraphics.gameObject.SetActive(true);


            if (!_audio[0].isPlaying) {
                _audio[0].Play();
            }

            // Testa se o laser está atingindo um alvo.
            RaycastHit2D hit = new RaycastHit2D();

            hit = Physics2D.Raycast(projectile_spawn[0].position, direction * Vector2.up, GameController.gameController.screenSize.y * 3, raycastMask);

            if (hit.collider != null) {

                _laserGraphics.SetPosition(0, projectile_spawn[0].position);
                _laserGraphics.SetPosition(1, hit.point + new Vector2(0, direction * 0.25f));

                EntityBase target = hit.collider.gameObject.GetComponent<EntityBase>();

                if (delayShots == 0) {

                    if (target != null)
                        target.Damage(damage, paralyse);

                    if (!infinite_ammo)
                        currentAmmo--;

                    delayShots += Time.deltaTime;
                }
            }
            else {

                _laserGraphics.SetPosition(0, projectile_spawn[0].position);
                _laserGraphics.SetPosition(1, projectile_spawn[0].position + new Vector3(0, direction * GameController.gameController.screenSize.y * 3, 0));
            }


        }
    }
}
