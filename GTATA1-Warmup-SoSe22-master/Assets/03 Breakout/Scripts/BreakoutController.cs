using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts
{
    public class BreakoutController : MonoBehaviour
    {
        [SerializeField] [Range(0, 1000)] private float speed;
        [SerializeField] [Range(0, 384)] private float pedalSize;
        [SerializeField] private List<Ball> balls;
        public Ball ballPrefab;
        private CapsuleCollider2D collider;
        private RectTransform rectTransform;
        private Rigidbody2D rigidBody;
        private int upgradeCollisionId;
        private bool smallBall = false;
        private bool bigBall = false;
        
        /// <summary>
        /// FixedUpdate is a Unity runtime function called *every physics* frame
        /// see: https://docs.unity3d.com/Manual/ExecutionOrder.html
        /// </summary>
        private void FixedUpdate()
        {
            // set the pedal graphic and collider size based on the pedal size property
            rectTransform.sizeDelta = new Vector2(pedalSize, rectTransform.sizeDelta.y);
            collider.size = new Vector2(pedalSize, collider.size.y);

            var transformPosition = transform.position;
            var pedalPosition = transformPosition;
            var mousePosition = Input.mousePosition;
            // calculate the distance on the horizontal axis between mouse and center of the pedal
            var mouseDistance = pedalPosition.x - mousePosition.x;
            var direction = Mathf.Sign(mouseDistance);

            var newPositionX = direction > 0
                ? Mathf.Max(mousePosition.x, pedalPosition.x - speed * Time.deltaTime)
                : Mathf.Min(mousePosition.x, pedalPosition.x + speed * Time.deltaTime);
            // updating the rigid body so the physics engine works correctly
            rigidBody.MovePosition(new Vector2(newPositionX, transformPosition.y));

            // remove all destroyed balls
            balls = balls.Where(x => x != null).ToList();

            // if there are no balls left, the game is lost
            if (balls.Count == 0)
            {
                // maybe this is a good entry point for a loss system, similarly no bricks -> win
                GameManager.Instance.SwitchState(GameManager.State.GAMEOVER);
            }
        }

        private void OnEnable()
        {
            // set some references once
            rigidBody = GetComponent<Rigidbody2D>();
            collider = GetComponent<CapsuleCollider2D>();
            rectTransform = GetComponent<RectTransform>();
            upgradeCollisionId = LayerMask.NameToLayer("Upgrade");
            var ball = Instantiate(ballPrefab, GameManager.Instance.playScene.transform);
            balls.Add(ball);
        }

        /// <summary>
        /// Collision event triggered from Unity when two colliders (in the physics collision layer matrix) collide with
        /// each other 
        /// </summary>
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == upgradeCollisionId)
            {
                var upgradeType = other.gameObject.GetComponent<Upgrade>().Type;

                // control code for upgrades
                switch (upgradeType)
                {
                    case UpgradeType.BiggerPedal:
                        pedalSize += 16;
                        break;
                    case UpgradeType.SmallerPedal:
                        pedalSize -= 16;
                        break;
                    case UpgradeType.BiggerBall:
                        if (smallBall)
                        {
                            UpdateBallsSize(balls, new Vector2(16, 16));
                            smallBall = false;
                        }
                        else
                        {
                            UpdateBallsSize(balls, new Vector2(32, 32));
                            bigBall = true;
                        }
                        break;
                    case UpgradeType.SmallerBall:
                        if (bigBall)
                        {
                            UpdateBallsSize(balls, new Vector2(16, 16));
                            bigBall = false;
                        }
                        else
                        {
                            UpdateBallsSize(balls, new Vector2(8, 8));
                            smallBall = true;
                        }                 
                        break;
                    case UpgradeType.ExtraBall:
                        var ball = Instantiate(balls[0], transform.parent);
                        ball.transform.position = balls[0].transform.position;
                        ball.RigidBody.velocity = Vector2.Reflect(balls[0].RigidBody.velocity, Vector2.up);
                        balls.Add(ball);
                        break;
                }

                pedalSize = Mathf.Clamp(pedalSize, 48, 384);
                Destroy(other.gameObject);
            }
        }

        /// <summary>
        /// update all available ball size based on the given vec2
        /// </summary>
        private void UpdateBallsSize(List<Ball> ballList, Vector2 vec2)
        {
            foreach (var ball in ballList)
            {
                ball.transform.GetComponent<RectTransform>().sizeDelta = vec2;
            }
        }
    }
}