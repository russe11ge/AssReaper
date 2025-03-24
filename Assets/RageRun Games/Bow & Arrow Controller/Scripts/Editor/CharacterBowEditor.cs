using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

namespace RageRunGames.BowArrowController
{
    public class CharacterBowEditor : EditorWindow
    {
        private Object characterObject;
        private Bow bowObject;

        private ObjectField characterField;
        private ObjectField bowField;

        [MenuItem("Window/RageRun Games/Character & Bow Setup")]
        public static void ShowWindow()
        {
            var window = GetWindow<CharacterBowEditor>();
            window.titleContent = new GUIContent("Character & Bow Setup");
            window.minSize = new Vector2(300, 150);
        }

        private void CreateGUI()
        {
       
            VisualElement root = rootVisualElement;

            ObjectField characterField = new ObjectField("Character")
            {
                objectType = typeof(GameObject),
                allowSceneObjects = true
            };
            characterField.RegisterValueChangedCallback(evt => characterObject = evt.newValue);
            characterField.tooltip =  "Drag one of the Bow Components from Prefab/Bow & Arrow, or use your own model (requires pre-configuration with the Bow Component).";
            root.Add(characterField);

            ObjectField bowField = new ObjectField("Bow")
            {
                objectType = typeof(Bow),
                allowSceneObjects = true
            };

            this.characterField = characterField;
            this.bowField = bowField;
            
            bowField.RegisterValueChangedCallback(evt => bowObject = (Bow)evt.newValue);
            bowField.tooltip =  "Drag one of the Bow Component, find it under Prefab/Bow & Arrow or use your own model (require pre-configuration with bow component)";
            root.Add(bowField);

            Button createButton = new Button(() => { CreateCharacterBow(characterObject, bowObject); })
            {
                text = "Create FPS Bow Controller"
            };
           
            root.Add(createButton);

            Button resetButton = new Button(() => { ResetData(characterField, bowField); })
            {
                text = "Reset Fields"
            };
            resetButton.tooltip = "";
            root.Add(resetButton);
            
            root.Add(new VisualElement { style = { height = 10} });
            
            VisualElement bulletPoints = new VisualElement();
            bulletPoints.Add(new Label("• Make sure the character object has an Animator Controller component."));
            bulletPoints.Add(new Label("• The bow should be pre-configured with the necessary bow components and data, except for IK handling."));
            bulletPoints.Add(new Label("• If you encounter any issues, feel free to contact me via email at ragerungames@gmail.com."));
            root.Add(bulletPoints);
        }

    private void CreateCharacterBow(Object character, Object bowObject1)
        {
            
            Camera camera = Camera.main;

            if (camera != null)
            {
                camera.gameObject.SetActive(false);
                Debug.Log("Disabling main camera in the scene as the FPS Controller comes with it's own camera.");
            }
            
            if (bowObject1 == null)
            {
                Debug.LogError("Character or Bow is missing");
                return;
            }

            GameObject characterGameObject = (GameObject)character;
            Bow bowGameObject = (Bow)bowObject1;


            if (bowGameObject.GetComponent<Bow>() == null)
            {
                Debug.LogError("Bow is missing Bow component");
                return;
            }


            GameObject characterPreset =  Instantiate(Resources.Load("CharacterPreset", typeof(GameObject))) as GameObject;

            Transform camTransform = characterPreset.transform.GetChild(0);
            Transform bobbingTransform = camTransform.Find("Bobbing Transform");

            Bow newBow = Instantiate(bowGameObject, bobbingTransform.Find("BowHolder")).GetComponent<Bow>();
            newBow.gameObject.SetActive(true);
            
            Transform dummyBot = camTransform.Find("KyleRobot");
            Transform rig1 = dummyBot.transform.Find("Rig 1");
            rig1.parent = null;


            if (dummyBot != null)
            {
                GameObject.DestroyImmediate(dummyBot.gameObject);
            }
            
            Rig rig = rig1.GetComponent<Rig>();
            
            Transform newCharacter = Instantiate(characterGameObject, camTransform).transform;
            newCharacter.transform.localPosition = new Vector3(0f, -1.6603f, -0.15f);

            Animator animator = newCharacter.GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogError("Animator Missing in the character model");
                DestroyImmediate(characterPreset);
                ResetData(characterField, bowField);
                return;
            }

            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("PlayerAnimController");
            
            TwoBoneIKConstraint leftHandIK = rig1.Find("Left Hand IK").GetComponent<TwoBoneIKConstraint>();
            leftHandIK.data.tip = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            leftHandIK.data.mid = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            leftHandIK.data.root = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);

            TwoBoneIKConstraint rightHandIK = rig1.Find("Right Hand IK").GetComponent<TwoBoneIKConstraint>();
            rightHandIK.data.tip = animator.GetBoneTransform(HumanBodyBones.RightHand);
            rightHandIK.data.mid = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            rightHandIK.data.root = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            
            RigBuilder rigBuilder = newCharacter.transform.AddComponent<RigBuilder>();
            rigBuilder.layers.Add(new RigLayer( rig ));
            
            rig1.transform.parent = newCharacter;

            characterPreset.transform.position = new Vector3(-30f, 0.5f, 0f);
        }

        private void ResetData(ObjectField characterField, ObjectField bowField)
        {
            characterObject = null;
            bowObject = null;
            characterField.value = null;
            bowField.value = null;
        }
    }

}