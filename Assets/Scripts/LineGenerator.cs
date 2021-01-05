using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    [SerializeField] float initRadius = 300f;
    [SerializeField] UnityEngine.UI.Button buttonSplit = default;
    [SerializeField] UnityEngine.UI.Button buttonMerge = default;
    [SerializeField] float minWidth = 0.018f;

    private LineRenderer line = default;
    private List<Vector3> positions = new List<Vector3>();
    private float lineWidth = 0.5f;

    public float InitRadius => initRadius;

    // Start is called before the first frame update
    void Start()
    {
        buttonSplit.onClick.AddListener(() => Split());
        buttonMerge.onClick.AddListener(() => Merge());

        line = GetComponent<LineRenderer>();
        positions.Clear();
        positions.Add(new Vector3(-initRadius, initRadius / 2, 0));
        positions.Add(new Vector3(initRadius, initRadius / 2, 0));
        line.SetPositions(positions.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Split()
    {
        float sqrt2 = Mathf.Sqrt(2f);
        int iterations = positions.Count - 1;
        int index = 1;
        bool foward = false;
        for (int i = 0; i < iterations; i++)
        {
            //Vector3 midPoint = Vector3.Lerp(positions[index-1], positions[index], 0.5f);
            Vector3 offset = positions[index] - positions[index - 1];
            offset = Quaternion.AngleAxis(45, foward ? Vector3.forward : Vector3.back) * offset;
            Vector3 offset2 = offset.normalized * offset.magnitude / sqrt2;
            positions.Insert(index, positions[index - 1] + offset2);
            index += 2;
            foward = !foward;
        }

        line.positionCount = positions.Count;
        line.SetPositions(positions.ToArray());
        lineWidth = lineWidth / 1.4f;
        line.widthMultiplier = Mathf.Max(minWidth, lineWidth);
    }

    public void Merge()
    {
        if (positions.Count <= 2) return;

        int iterations = (positions.Count - 1) / 2;
        int index = 1;
        for (int i = 0; i < iterations; i++)
        {
            positions.RemoveAt(index);
            index += 1;
        }

        line.positionCount = positions.Count;
        line.SetPositions(positions.ToArray());
        lineWidth = lineWidth * 1.4f;
        line.widthMultiplier = Mathf.Max(minWidth, lineWidth);
    }
}
