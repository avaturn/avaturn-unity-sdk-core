using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Avaturn
{
    [RequireComponent(typeof(Animator))]
    public class PrepareAvatar : MonoBehaviour
    {
        //Receives a downloaded model and converts it into mecanim avatar.

        [SerializeField] private Animator _animator;
        [SerializeField] private DownloadAvatarEvents events;
        [Tooltip("Clear root gameObject out of prev avatar gameObjects. Start from that child index")]
        [SerializeField] private int _clearRootFromIndex;
        [Tooltip("If you didn't save your avatar asset and didn't appoint it to the Animator, use this")]
        [SerializeField] private bool _prepareOnStart;
    
        private void Start()
        {
            events.OnSuccess += PrepareModel;
        
            if (_prepareOnStart)
            {
                _animator.avatar = HumanoidAvatarBuilder.Build(gameObject);
            }
        }
    
        public void SetAnimator(UnityEngine.Avatar avatar)
        {
            _animator = GetComponent<Animator>();
            _animator.avatar = avatar;
        }

        private async void PrepareModel(Transform downloadedModel)
        {
            if (_animator.applyRootMotion)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }

            downloadedModel.gameObject.SetActive(false);

            //delete prev model and skeleton
            for (int i = _clearRootFromIndex; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            await Task.Yield();
        
            // Go trough hierarchy and move model from downloaded avatar to base
            var root = downloadedModel.transform.GetChild(0);
        
            if (!root)
            {
                Debug.LogWarning("Prepare failed. Can't find root object");
                return;
            }

            // Go trough hierarchy and move model from downloaded avatar to base
            var childCount = root.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = root.GetChild(0);
                child.SetParent(transform);
            }
            _animator.avatar = HumanoidAvatarBuilder.Build(gameObject);
        
            await Task.Yield();
            Destroy(root.gameObject);
        }
    }

    public static class HumanoidAvatarBuilder
    {
        public static UnityEngine.Avatar Build(GameObject root)
        {
            HumanDescription description = AvatarUtils.CreateHumanDescription(root);
            UnityEngine.Avatar avatar = AvatarBuilder.BuildHumanAvatar(root.gameObject, description);
            avatar.name = root.name;
            return avatar;
        }
    }


    public class AvatarUtils
    {
        public static Dictionary<string, string> HumanSkeletonNames = new Dictionary<string, string>()
        {
            {"Spine1", "Chest"},
            {"Head", "Head" },
            {"Hips", "Hips" },
            {"LeftHandIndex3", "Left Index Distal" },
            {"LeftHandIndex2", "Left Index Intermediate" },
            {"LeftHandIndex1", "Left Index Proximal" },
            {"LeftHandPinky3", "Left Little Distal" },
            {"LeftHandPinky2", "Left Little Intermediate" },
            {"LeftHandPinky1", "Left Little Proximal" },
            {"LeftHandMiddle3", "Left Middle Distal" },
            {"LeftHandMiddle2", "Left Middle Intermediate" },
            {"LeftHandMiddle1", "Left Middle Proximal" },
            {"LeftHandRing3", "Left Ring Distal" },
            {"LeftHandRing2", "Left Ring Intermediate" },
            {"LeftHandRing1", "Left Ring Proximal" },
            {"LeftHandThumb3", "Left Thumb Distal" },
            {"LeftHandThumb2", "Left Thumb Intermediate" },
            {"LeftHandThumb1", "Left Thumb Proximal" },
            {"LeftFoot", "LeftFoot" },
            {"LeftHand", "LeftHand" },
            {"LeftForeArm", "LeftLowerArm" },
            {"LeftLeg", "LeftLowerLeg" },
            {"LeftShoulder", "LeftShoulder" },
            {"LeftToeBase", "LeftToes" },
            {"LeftArm", "LeftUpperArm" },
            {"LeftUpLeg", "LeftUpperLeg" },
            {"Neck", "Neck" },
            {"RightHandIndex3", "Right Index Distal" },
            {"RightHandIndex2", "Right Index Intermediate" },
            {"RightHandIndex1", "Right Index Proximal" },
            {"RightHandPinky3", "Right Little Distal" },
            {"RightHandPinky2", "Right Little Intermediate" },
            {"RightHandPinky1", "Right Little Proximal" },
            {"RightHandMiddle3", "Right Middle Distal" },
            {"RightHandMiddle2", "Right Middle Intermediate" },
            {"RightHandMiddle1", "Right Middle Proximal" },
            {"RightHandRing3", "Right Ring Distal" },
            {"RightHandRing2", "Right Ring Intermediate" },
            {"RightHandRing1", "Right Ring Proximal" },
            {"RightHandThumb3", "Right Thumb Distal" },
            {"RightHandThumb2", "Right Thumb Intermediate" },
            {"RightHandThumb1", "Right Thumb Proximal" },
            {"RightFoot", "RightFoot" },
            {"RightHand", "RightHand" },
            {"RightForeArm", "RightLowerArm" },
            {"RightLeg", "RightLowerLeg" },
            {"RightShoulder", "RightShoulder" },
            {"RightToeBase", "RightToes" },
            {"RightArm", "RightUpperArm" },
            {"RightUpLeg", "RightUpperLeg" },
            {"Spine", "Spine" },
            {"Spine2", "UpperChest" },
        };

        /// <summary>
        /// Create a HumanDescription out of an avatar GameObject. 
        /// The HumanDescription is what is needed to create an Avatar object
        /// using the AvatarBuilder API. This function takes care of 
        /// creating the HumanDescription by going through the avatar's
        /// hierarchy, defining its T-Pose in the skeleton, and defining
        /// the transform/bone mapping in the HumanBone array. 
        /// </summary>
        /// <param name="avatarRoot">Root of your avatar object</param>
        /// <returns>A HumanDescription which can be fed to the AvatarBuilder API</returns>
        public static HumanDescription CreateHumanDescription(GameObject avatarRoot)
        {
            HumanDescription description = new HumanDescription()
            {
                armStretch = 0.05f,
                feetSpacing = 0f,
                hasTranslationDoF = false,
                legStretch = 0.05f,
                lowerArmTwist = 0.5f,
                lowerLegTwist = 0.5f,
                upperArmTwist = 0.5f,
                upperLegTwist = 0.5f,
                skeleton = CreateSkeleton(avatarRoot),
                human = CreateHuman(avatarRoot),
            };
            return description;
        }

        //Create a SkeletonBone array out of an Avatar GameObject
        //This assumes that the Avatar as supplied is in a T-Pose
        //The local positions of its bones/joints are used to define this T-Pose
        private static SkeletonBone[] CreateSkeleton(GameObject avatarRoot)
        {
            List<SkeletonBone> skeleton = new List<SkeletonBone>();

            Transform[] avatarTransforms = avatarRoot.GetComponentsInChildren<Transform>();
            foreach (Transform avatarTransform in avatarTransforms)
            {
                SkeletonBone bone = new SkeletonBone()
                {
                    name = avatarTransform.name,
                    position = avatarTransform.localPosition,
                    rotation = avatarTransform.localRotation,
                    scale = avatarTransform.localScale
                };

                skeleton.Add(bone);
            }
            return skeleton.ToArray();
        }

        //Create a HumanBone array out of an Avatar GameObject
        //This is where the various bones/joints get associated with the
        //joint names that Unity understands. This is done using the
        //static dictionary defined at the top. 
        private static HumanBone[] CreateHuman(GameObject avatarRoot)
        {
            List<HumanBone> human = new List<HumanBone>();

            Transform[] avatarTransforms = avatarRoot.GetComponentsInChildren<Transform>();
            foreach (Transform avatarTransform in avatarTransforms)
            {
                if (HumanSkeletonNames.TryGetValue(avatarTransform.name, out string humanName))
                {
                    HumanBone bone = new HumanBone
                    {
                        boneName = avatarTransform.name,
                        humanName = humanName,
                        limit = new HumanLimit()
                    };
                    bone.limit.useDefaultValues = true;

                    human.Add(bone);
                }
            }
            return human.ToArray();
        }
    }
}