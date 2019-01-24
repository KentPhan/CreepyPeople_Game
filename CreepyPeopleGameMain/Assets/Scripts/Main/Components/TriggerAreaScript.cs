using Assets.Scripts.Main.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Main.Components
{
    public class TriggerAreaScript : MonoBehaviour
    {
        [System.Serializable]
        public class AudioTrigger
        {
            public AudioSource AudioSource;
            public float Delay;
        }

        [System.Serializable]
        public class TransformTriggers
        {
            public GameObject Object;
            public Transform NewTransform;
        }


        public List<AudioTrigger> AudiosToTrigger;

        [SerializeField]
        public List<EnemyScript> EnemiesToTrigger;


        public List<TransformTriggers> MovementToTrigger;

        public bool Triggered = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void OnTriggerEnter()
        {
            if (!Triggered)
            {
                // Trigger Audio
                foreach (AudioTrigger l_Audio in AudiosToTrigger)
                {
                    l_Audio.AudioSource.PlayDelayed(l_Audio.Delay);
                }

                // Enemies
                foreach (EnemyScript l_Enemy in EnemiesToTrigger)
                {
                    if (l_Enemy.IsDormant())
                        l_Enemy.WakeUp();
                }

                // Transforms
                foreach (TransformTriggers l_Transform in MovementToTrigger)
                {
                    l_Transform.Object.GetComponent<Rigidbody>().MovePosition(l_Transform.NewTransform.position);
                }

                Triggered = true;
            }
        }

    }
}
