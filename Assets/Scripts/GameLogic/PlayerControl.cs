using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Test
{
    public class PlayerControl : MonoBehaviour
    {
        public static GameObject player;

        public float speed = 1f;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        private void Start()
        {

            AVGSaveSystem.instance.onLoad.AddListener(OnLoad);
            AVGSaveSystem.instance.onSave.AddListener(OnSave);
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.D) && !cantMoveRight)
            {
                this.gameObject.transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * speed);
            }

            if (Input.GetKey(KeyCode.A) && !cantMoveLeft)
            {
                this.gameObject.transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * speed);
            }
        }
        [SerializeField]
        private bool cantMoveLeft = false;
        [SerializeField]
        private bool cantMoveRight = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Obstacle")
            {
                if (collision.gameObject.transform.position.x < this.transform.position.x)
                {
                    cantMoveLeft = true;
                }
                else
                {
                    cantMoveRight = true;
                }
            }
        }


        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Obstacle")
            {
                cantMoveRight = false;
                cantMoveLeft = false;
            }
        }

        public void OnSave()
        {
            ES3.Save<float>("PlayerX", this.gameObject.transform.position.x);
            ES3.Save<float>("PlayerY", this.gameObject.transform.position.y);
        }
        public void OnLoad()
        {
            Debug.Log("load");
            this.gameObject.transform.position = 
                new Vector3(
                    ES3.Load<float>("PlayerX", 0),
                    ES3.Load<float>("PlayerY", 0), 
                    0);
        }
    }
}