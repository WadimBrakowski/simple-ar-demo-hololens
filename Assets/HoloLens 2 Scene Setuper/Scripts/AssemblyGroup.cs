using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTK.Tutorials.GettingStarted;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace HoloLens_2_Scene_Setuper
{
    public class AssemblyGroup : MonoBehaviour
    {
        // Member
        [SerializeField] private GameObject arWorkingSpace;
        [SerializeField] private Vector3 arWorkingSpacePosition = new Vector3(0, -.6f, 2f);
        private GameObject _loadedObjectFile;

        private GameObject _assemblyGroup;
        private GameObject _baseObject;
        private GameObject _parentForInteractableParts;
        private GameObject _placementHintsParent;
        private List<GameObject> _listToArray;
        private GameObject[] _arrayOfPlacementHints;

        private Vector3 _originPosition;
        private List<GameObject> _listOfParts;
        private List<GameObject> _listOfBaseObjectParts;
        private GameObject _part;
        
        private Vector3 _position;
        
        private Vector3 _parentPosition;
        private GridObjectCollection _gridObjectCollection;
        
        private Shader _shader;

        [SerializeField] private TextMeshPro namePlate;
        // Components
        private PlacementHintsController _placementHintsController;
        private SolverHandler _solverHandler;
        private PartAssemblyController _partAssemblyController;
        private NearInteractionGrabbable _nearInteractionGrabbable;
        private ObjectManipulator _objectManipulator;
        private ConstraintManager _constraintManager;
        private ExplodeViewController _explodeViewController;

        private MeshCollider _meshCollider;
        private BoxCollider _boxCollider;
        private AudioSource _audioSource;
        [SerializeField] private AudioClip audioClip;
        private ToolTipSpawner _toolTipSpawner;
        private Mesh _mesh;
        private MeshFilter _meshFilter;

        private Renderer _renderer;
        private Bounds _bounds;
        [SerializeField] private Material material;

        
        //Properties
        public GameObject LoadedObjectFile
        {
            get
            {
                return _loadedObjectFile;
            }
            set
            {
                _loadedObjectFile = value;
                //ScaleObjects();
                //_loadedObjectFile.transform.localScale = new Vector3(.01f, .01f, .01f);
                
                arWorkingSpace = Instantiate(arWorkingSpace, arWorkingSpacePosition,Quaternion.identity);
                CreateAssemblyGroup();

                InstantiateBaseObject();


                InstantiateParts();


                InstantiatePlacementHints();
                _baseObject.SetActive(false);

                SetHintsAsLocationToPlaceInAssemblyController();

                _assemblyGroup.transform.GetComponent<BoxCollider>().size =
                    InvertBoxColliderSize(_parentForInteractableParts.transform.GetChild(0).localScale);


            }
        }

        

        private void InstantiatePlacementHints()
        {
            
            _placementHintsParent = Instantiate(_baseObject, _assemblyGroup.transform);
            _placementHintsParent.name = "Placement HInts";
            
            _arrayOfPlacementHints = new GameObject[_placementHintsParent.transform.childCount];
            _listToArray = new List<GameObject>();


            foreach (Transform value in _placementHintsParent.transform)
            {
                GameObject hint = value.GameObject();
                _renderer = hint.GetComponentInChildren<Renderer>();
                _renderer.material = material;
                _listToArray.Add(hint);
            }

            _arrayOfPlacementHints = _listToArray.ToArray();
            _placementHintsController = _placementHintsParent.GetComponentInParent<PlacementHintsController>();
            _placementHintsController.PlacementHints = _arrayOfPlacementHints;
            
        }

        private void InstantiateParts()
        {
            _parentForInteractableParts = Instantiate(_baseObject, arWorkingSpace.transform);
            _parentForInteractableParts.name = "Parts";
            
            _parentForInteractableParts.AddComponent<GridObjectCollection>();
            _gridObjectCollection = _parentForInteractableParts.GetComponent<GridObjectCollection>();
            _gridObjectCollection.SortType = CollationOrder.Alphabetical;
            _gridObjectCollection.Layout = LayoutOrder.Horizontal;
            _gridObjectCollection.CellWidth = .25f;
            _gridObjectCollection.Distance = .38f;
            _gridObjectCollection.UpdateCollection();
            
            foreach (Transform part in _parentForInteractableParts.transform)
            {
                GameObject newPart = part.GameObject();
                _mesh = newPart.GetComponentInChildren<MeshFilter>().sharedMesh;
                _boxCollider = newPart.transform.AddComponent<BoxCollider>();
                
                var newPartLocalScale = newPart.transform.GetChild(0).localScale;

                _boxCollider.size = InvertBoxColliderSize(newPartLocalScale);
                
                _constraintManager = newPart.AddComponent<ConstraintManager>();
                _objectManipulator = newPart.AddComponent<ObjectManipulator>();
                _nearInteractionGrabbable = newPart.AddComponent<NearInteractionGrabbable>();
                _partAssemblyController = newPart.AddComponent<PartAssemblyController>();

                _partAssemblyController.LocationToPlace = newPart.transform;
                
                
                _renderer = newPart.GetComponentInChildren<Renderer>();
                _renderer.material = new Material(Shader.Find("Standard"));
            }
        }

        private void InstantiateBaseObject()
        {
            
            
            _listOfBaseObjectParts = new List<GameObject>();
            _baseObject = _loadedObjectFile;
            _baseObject.name = "Base Object";

            foreach (Transform loadedObject in _baseObject.transform)
            {
                GameObject gameObject = loadedObject.gameObject;
                _listOfBaseObjectParts.Add(gameObject);
                ScaleObjects(gameObject);
            }

            foreach (GameObject obj in _listOfBaseObjectParts)
            {
                GameObject parent = new GameObject(obj.name+"_parent");

                obj.transform.parent = parent.transform;

                parent.transform.parent = _loadedObjectFile.transform;

                _renderer = obj.transform.GetComponent<Renderer>();
                _bounds = _renderer.bounds;

                if (obj.transform.position != _bounds.center)
                {
                    Vector3 deltaPosition = _bounds.center - obj.transform.position;

                    parent.transform.position += deltaPosition;
                    obj.transform.position -= deltaPosition;
                }
            }
            
            _baseObject.transform.parent = _assemblyGroup.transform;
            _baseObject.transform.position = arWorkingSpace.transform.position;
            
            
        }

        

        private void CreateAssemblyGroup()
        {
            _assemblyGroup = new GameObject("AssemblyGroup",
                typeof(BoxCollider),
                typeof(PlacementHintsController),
                typeof(SolverHandler),
                typeof(NearInteractionGrabbable),
                typeof(ObjectManipulator))
            {
                transform =
                {
                    parent = arWorkingSpace.transform,
                    position = arWorkingSpace.transform.position
                }
            };


            _partAssemblyController =_assemblyGroup.AddComponent<PartAssemblyController>();
            _partAssemblyController.LocationToPlace = _assemblyGroup.transform;

            //namePlate = _assemblyGroup.transform.GetComponentInChildren<TextMeshPro>();
            //namePlate.text = _loadedObjectFile.name;
        }

        private void ScaleObjects(GameObject gameObject)
        {
            float arWorkingPlaceScale = arWorkingSpace.transform.localScale.x;
            _mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

            float factor = arWorkingPlaceScale / _mesh.bounds.size.x;

            gameObject.transform.localScale *= factor / 5;
        }

        private Vector3 InvertBoxColliderSize(Vector3 scale)
        {
            float x, y, z;
            if (scale.x < 0)
            {
                x = scale.x * -1/2;
            }
            else
            {
                x = scale.x/2;
            }
            
            if (scale.y < 0)
            {
                y = scale.y * -1/2;
            }
            else
            {
                y = scale.y/2;
            }
            
            if (scale.z < 0)
            {
                z = scale.z * -1/2;
            }
            else
            {
                z = scale.z/2;
            }
            
            return new Vector3(x,y,z);
        }
        
        private void SetHintsAsLocationToPlaceInAssemblyController()
        {
            foreach (var hint in _arrayOfPlacementHints)
            {
                foreach (Transform part in _parentForInteractableParts.transform)
                {
                    if (hint.name == part.name)
                    {
                        part.GetComponent<PartAssemblyController>().LocationToPlace = hint.transform;
                    }
                }
            }
        }

    }
}
