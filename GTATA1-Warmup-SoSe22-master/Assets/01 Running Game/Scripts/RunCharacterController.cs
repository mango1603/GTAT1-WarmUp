using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Controls the movement of the Character
    /// </summary>
    public class RunCharacterController : MonoBehaviour
    {
        public Transform Transform => character;
        public SpriteRenderer CharacterSprite => characterSprite;

        /// <summary>
        /// Since the Character controller takes responsibility for triggering Input events, it also emits an
        /// event when it does so
        /// </summary>
        public Action onJump;

        [SerializeField] private float jumpHeight;
        [SerializeField] private float jumpDuration;

        /// <summary>
        /// Unity handles Arrays and Lists in the inspector correctly (but not Maps, Dictionaries or other Collections)
        /// </summary>
        [SerializeField] private KeyCode[] jumpKeys;

        /// <summary>
        /// We don't require anything else from the Character than its transform
        /// </summary>
        [SerializeField] private Transform character;

        /// <summary>
        /// List of Sprite assets for the transformations & animations
        /// </summary>
        [SerializeField] private SpriteRenderer characterSprite;
        [SerializeField] private Sprite[] moveSpriteArrayBeige;
        [SerializeField] private Sprite jumpSpriteBeige;
        [SerializeField] private Sprite[] moveSpriteArrayBlue;
        [SerializeField] private Sprite jumpSpriteBlue;
        [SerializeField] private Sprite[] moveSpriteArrayGreen;
        [SerializeField] private Sprite jumpSpriteGreen;
        [SerializeField] private Sprite[] moveSpriteArrayPink;
        [SerializeField] private Sprite jumpSpritePink;
        [SerializeField] private Sprite[] moveSpriteArrayYellow;
        [SerializeField] private Sprite jumpSpriteYellow;

        [SerializeField] private AnimationCurve jumpPosition;

        /// <summary>
        /// Audio field
        /// </summary>
        [SerializeField] private AudioClip jumpAudio;
        [SerializeField] private AudioClip fallAudio;


        private bool canJump = true;
        private bool move = true;
        private int colorIndex = 0;
        private IEnumerator e;

        /// <summary>
        /// Update is a Unity runtime function called *every rendered* frame before Rendering happens
        /// see: https://docs.unity3d.com/Manual/ExecutionOrder.html
        /// </summary>
        private void Update()
        {
            if (!canJump)
            {
                return;
            }
            
            // Start moving animation while the character is not jumping 
            if (move)
            {
                //making sure this only happens once
                StopAllCoroutines();
                move = false;
                e = MoveRoutine();
                StartCoroutine(e);
            }
            
            // here the input event counts - if there is any button pressed that were defined as jump keys, trigger a jump
            if (jumpKeys.Any(x => Input.GetKeyDown(x)))
            {
                //Stop moving coroutines
                StopAllCoroutines();
                //Switch to jump sprite and play the jump sound once
                characterSprite.sprite = GetJumpSprite();
                SoundManager.Instance.PlaySoundOnce(jumpAudio);
                //Switch the character color after a jump is performed
                SwitchColor();

                // first we disable the jump, then start the Coroutine that handles the jump and invoke the event
                canJump = false;
                StartCoroutine(JumpRoutine());
                onJump?.Invoke();
            }
        }

        /// <summary>
        /// OnDrawGizmosSelected is a Unity editor function called when the attached GameObject is selected and used to
        /// display debugging information in the Scene view
        /// see: https://docs.unity3d.com/Manual/ExecutionOrder.html
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            var upScale = transform.lossyScale;
            upScale.Scale(transform.up);
            Gizmos.DrawLine(transform.position, Vector3.up * jumpHeight * upScale.magnitude);
        }

        /// <summary>
        /// Handles the jump of a character
        /// 
        /// To be used in an Coroutine, this function is a generator (return IEnumerator) and has special syntactic
        /// sugar with "yield return"
        /// </summary>
        private IEnumerator JumpRoutine()
        {
            // the time this coroutine runs
            var totalTime = 0f;
            // low position is assumed to be a (0, 0, 0)
            var highPosition = character.up * jumpHeight;
            while (totalTime < jumpDuration)
            {
                totalTime += Time.deltaTime;
                // what's the normalized time [0...1] this coroutine runs at
                var sampleTime = totalTime / jumpDuration;
                // Lerp is a Linear Interpolation between a...b based on a value between 0...1
                character.localPosition = Vector3.Lerp(Vector3.zero, highPosition, jumpPosition.Evaluate(sampleTime));
                // we enable jumping again after we're almost done to remove some "stuck" behaviour when landing down
                if (sampleTime > 0.95f)
                {
                    //play the fall/transformation sound once and adjust the state variable
                    canJump = true;
                    move = true;
                    SoundManager.Instance.PlaySoundOnce(fallAudio);
                }

                // yield return null waits a single frame
                yield return null;
            }
        }

        /// <summary>
        /// Handle the move sprite array
        /// Switching between the element inside the array
        /// </summary>
        private IEnumerator MoveRoutine()
        {
            while (true)
            {
                var sprites = GetMoveSprites();
                var currentIndex = Array.FindIndex(sprites, sprite => sprite == characterSprite.sprite);
                characterSprite.sprite = currentIndex == -1
                    ? sprites[1]
                    : GetNextElement(sprites, currentIndex);
                yield return new WaitForSeconds(0.5f);
            }
        }

        /// <summary>
        /// Get the move sprite array based on the current color index 
        /// </summary>
        private Sprite[] GetMoveSprites()
        {
            return colorIndex switch
            {
                0 => moveSpriteArrayBeige,
                1 => moveSpriteArrayBlue,
                2 => moveSpriteArrayGreen,
                3 => moveSpriteArrayPink,
                4 => moveSpriteArrayYellow,
                _ => moveSpriteArrayBeige
            };
        }

        /// <summary>
        /// Get the jump sprite based on the current color index
        /// </summary>
        private Sprite GetJumpSprite()
        {
            return colorIndex switch
            {
                0 => jumpSpriteBeige,
                1 => jumpSpriteBlue,
                2 => jumpSpriteGreen,
                3 => jumpSpritePink,
                4 => jumpSpriteYellow,
                _ => jumpSpriteBeige
            };
        }

        /// <summary>
        /// Adjusting the color index when this method is called
        /// </summary>
        private void SwitchColor()
        {
            if (colorIndex < 4)
                colorIndex++;
            else
                colorIndex = 0;
        }

        /// <summary>
        /// Utility Method
        /// Get the next element of the given sprite array and the current index
        /// </summary>
        private Sprite GetNextElement(Sprite[] array, int index)
        {
            if (index == array.Length - 1)
                index = 0;
            else
                index++;
            return array[index];
        }
    }
}