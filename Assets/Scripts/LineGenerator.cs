using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    [SerializeField] float initRadius = 300f;
    [SerializeField] UnityEngine.UI.Button buttonSplit = default;
    [SerializeField] UnityEngine.UI.Button buttonMerge = default;
    [SerializeField] UnityEngine.UI.Text labelStep = default;
    [SerializeField] float minWidth = 0.018f;
    [SerializeField] float fadeTime = 0.4f;

    private LineRenderer line = default;
    private List<Vector3> positions = new List<Vector3>();
    private List<Vector3> prevPositions = new List<Vector3>();
    private float lineWidth = 0.5f;
    private int iterationStep = 1;
    private bool isInFade = false;

    public float InitRadius => initRadius;

    // Start is called before the first frame update
    void Start()
    {
        buttonSplit.onClick.AddListener(() => Split());
        buttonMerge.onClick.AddListener(() => Merge());

        line = GetComponent<LineRenderer>();
        positions.Clear();
        positions.Add(new Vector3(-initRadius, initRadius / 2, 0));
        positions.Add(new Vector3(0, -initRadius / 2, 0));
        positions.Add(new Vector3(initRadius, initRadius / 2, 0));
        line.positionCount = positions.Count;
        line.SetPositions(positions.ToArray());

        labelStep.text = "Iteration : " + iterationStep.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Split()
    {
        if (isInFade || iterationStep >= 15) return;

        iterationStep++;

        float sqrt2 = Mathf.Sqrt(2f);
        int iterations = positions.Count - 1;
        int index = 1;
        bool foward = false;
        prevPositions = new List<Vector3>(positions); // clone list

        for (int i = 0; i < iterations; i++)
        {
            Vector3 midPoint = Vector3.Lerp(positions[index-1], positions[index], 0.5f);
            prevPositions.Insert(index, midPoint);

            Vector3 offset = positions[index] - positions[index - 1];
            offset = Quaternion.AngleAxis(45, foward ? Vector3.forward : Vector3.back) * offset;
            Vector3 offset2 = offset.normalized * offset.magnitude / sqrt2;
            positions.Insert(index, positions[index - 1] + offset2);
            index += 2;
            foward = !foward;
        }

        isInFade = true;
        StartCoroutine(ProcSplit());

        labelStep.text = "Iteration : " + iterationStep.ToString();
    }

    IEnumerator ProcSplit()
    {
        line.positionCount = prevPositions.Count;
        line.SetPositions(prevPositions.ToArray());

        List<Vector3> animatePositions = new List<Vector3>(prevPositions); // clone list
        float w = 0f;
        int iterations = (positions.Count - 1) / 2;

        while (w < 1f)
        {
            int index = 1;
            for (int i = 0; i < iterations; i++)
            {
                animatePositions[index] = Vector3.Lerp(prevPositions[index], positions[index], w);
                index += 2;
            }

            line.SetPositions(animatePositions.ToArray());
            yield return null;
            w += Time.deltaTime / fadeTime;
        }

        line.positionCount = positions.Count;
        line.SetPositions(positions.ToArray());
        lineWidth = lineWidth / 1.4f;
        line.widthMultiplier = Mathf.Max(minWidth, lineWidth);
        isInFade = false;
    }

    public void Merge()
    {
        if (isInFade || iterationStep <= 1) return;

        iterationStep--;

        prevPositions = new List<Vector3>(positions); // clone list

        int iterations = (positions.Count - 1) / 2;
        int index = 1;
        for (int i = 0; i < iterations; i++)
        {
            positions[index] = Vector3.Lerp(positions[index - 1], positions[index + 1], 0.5f);
            index += 2;
        }

        isInFade = true;
        StartCoroutine(ProcMerge());
    }

    IEnumerator ProcMerge()
    {
        line.positionCount = prevPositions.Count;
        line.SetPositions(prevPositions.ToArray());

        List<Vector3> animatePositions = new List<Vector3>(prevPositions); // clone list
        float w = 0f;
        int iterations = (positions.Count - 1) / 2;

        while (w < 1f)
        {
            int index = 1;
            for (int i = 0; i < iterations; i++)
            {
                animatePositions[index] = Vector3.Lerp(prevPositions[index], positions[index], w);
                index += 2;
            }

            line.SetPositions(animatePositions.ToArray());
            yield return null;
            w += Time.deltaTime / fadeTime;
        }

        EndMerge();

        isInFade = false;
        labelStep.text = "Iteration : " + iterationStep.ToString();
    }

    void EndMerge()
    {
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
