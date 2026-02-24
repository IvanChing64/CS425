using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField] private float cameraSpeed = 20f;
    [SerializeField] private float snapSpeed = 0.2f;

    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;


    private Transform mainCameraTransform;
    private Camera camComponent;
    private Vector3 homePosition;
    private float homeZoom;
    private bool isCamInitialized = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
        camComponent = Camera.main;
    }

    void Update()
    {
        if(GameManager.Instance.gameState != GameState.PlayerTurn)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleDeselect();
        }

        HandleCamera();
        HandleCameraZoom();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SnapBack());
        }
    }

    public void InitializeCamera(Vector3 pos, float initialZoom)
    {
        homePosition = pos;
        homeZoom = initialZoom;
        isCamInitialized = true;

    }

    public void HandleDeselect()
    {
        if(combatUIManager.Instance != null && combatUIManager.Instance.IsCombatMenuOpen)
        {
            return;
        }

        if(UnitManager.Instance.SelectedPlayer != null)
        {
            Debug.Log("Deselecting Unit: " + UnitManager.Instance.SelectedPlayer.name);

            ClearMovementHighlights();

            foreach (Tile t in GridManager.Instance.GetNeighborsOf(UnitManager.Instance.SelectedPlayer.OccupiedTile))
            {
                if (t.isWalkable) t.ShowHighlight(false, Tile.nonwalkableColor);
            }

            CardManager.instance.DeselectCard();
            UnitManager.Instance.SelectedPlayer.GetComponent<HandManager>().ToggleHandVisibility(false);
            UnitManager.Instance.SetSelectedPlayer(null);

            if(CardManager.instance != null){
                CardManager.instance.SetSelectedPlayer(null);
            }
            if(combatUIManager.Instance != null)
            {
                combatUIManager.Instance.hideCombatOption();
            }
        }
    }

    private void ClearMovementHighlights()
    {
        var selectedPlayer = UnitManager.Instance.SelectedPlayer;
        if(selectedPlayer != null)
        {
            List<Tile> tilesInRange = selectedPlayer.GetTilesInMoveRange();
            foreach (Tile t in tilesInRange)
            {
                t.ShowHighlight(false, Tile.nonwalkableColor);
            }
            selectedPlayer.OccupiedTile.ShowHighlight(false, Tile.nonwalkableColor);
        }
    }

    private void HandleCamera()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if(x != 0 || y != 0)
        {
            Vector3 move = new Vector3(x, y, 0).normalized;
            mainCameraTransform.position += move * cameraSpeed * Time.deltaTime;
        }
    }

    private void HandleCameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll != 0)
        {
            float newZoom = camComponent.orthographicSize - (scroll * zoomSpeed);
            camComponent.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
    }

    private IEnumerator SnapBack()
    {
        if (!isCamInitialized)
        {
            yield break;
        }
        float elapsed = 0;
        Vector3 startPos = mainCameraTransform.position;
        float startZoom = camComponent.orthographicSize;

        while(elapsed < snapSpeed)
        {
            mainCameraTransform.position = Vector3.Lerp(startPos, homePosition, elapsed / snapSpeed);
            camComponent.orthographicSize = Mathf.Lerp(startZoom, homeZoom, elapsed / snapSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCameraTransform.position = homePosition;
        camComponent.orthographicSize = homeZoom;
    }
}
