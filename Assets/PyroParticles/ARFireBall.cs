using UnityEngine;
using System.Collections;

namespace DigitalRuby.PyroParticles
{
    public class ARFireBall : MonoBehaviour
    {
        public GameObject[] Prefabs;
        public Light Sun;

        private GameObject currentPrefabObject;
        private FireBaseScript currentPrefabScript;
        private int currentPrefabIndex;
        private enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        private RotationAxes axes = RotationAxes.MouseXAndY;
        private float sensitivityX = 15F;
        private float sensitivityY = 15F;
        private float minimumX = -360F;
        private float maximumX = 360F;
        private float minimumY = -60F;
        private float maximumY = 60F;
        private float rotationX = 0F;
        private float rotationY = 0F;
        private Quaternion originalRotation;

        private void UpdateMovement()
        {
            float speed = 5.0f * Time.deltaTime;
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }

            return Mathf.Clamp(angle, min, max);
        }

        private void BeginEffect()
        {
            Vector3 pos;
            float yRot = transform.rotation.eulerAngles.y;
            Vector3 forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * Vector3.forward;
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            Vector3 up = transform.up;
            Quaternion rotation = Quaternion.identity;
            currentPrefabObject = GameObject.Instantiate(Prefabs[currentPrefabIndex]);
            currentPrefabScript = currentPrefabObject.GetComponent<FireConstantBaseScript>();

            if (currentPrefabScript == null)
            {
                // temporary effect, like a fireball
                currentPrefabScript = currentPrefabObject.GetComponent<FireBaseScript>();
                if (currentPrefabScript.IsProjectile)
                {
                    // set the start point near the player
                    rotation = transform.rotation;
                    pos = transform.position + forward + right;
                }
                else
                {
                    // set the start point in front of the player a ways
                    pos = transform.position + (forwardY * 10.0f);
                }
            }
            else
            {
                // set the start point in front of the player a ways, rotated the same way as the player
                pos = transform.position + (forwardY * 5.0f);
                rotation = transform.rotation;
                pos.y = 0.0f;
            }

            FireProjectileScript projectileScript = currentPrefabObject.GetComponentInChildren<FireProjectileScript>();
            if (projectileScript != null)
            {
                // make sure we don't collide with other friendly layers
                projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer("FriendlyLayer"));
            }

            currentPrefabObject.transform.position = pos;
            currentPrefabObject.transform.rotation = rotation;
        }

        public void ClickFireButton1() {
          currentPrefabIndex = 0;
          StartCurrent();
        }

        public void ClickFireButton2() {
          currentPrefabIndex = 1;
          StartCurrent();
        }

        public void StartCurrent()
        {
            StopCurrent();
            BeginEffect();
        }

        private void StopCurrent()
        {
            // if we are running a constant effect like wall of fire, stop it now
            if (currentPrefabScript != null && currentPrefabScript.Duration > 10000)
            {
                currentPrefabScript.Stop();
            }
            currentPrefabObject = null;
            currentPrefabScript = null;
        }

        private void Start()
        {
            originalRotation = transform.localRotation;
						Camera.main.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        }
    }
}
