using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneOperator : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Button buttonSplit = default;
    [SerializeField] UnityEngine.UI.Button buttonMerge = default;
    [SerializeField] UnityEngine.UI.Text labelIteration = default;
    [SerializeField] LineGenerator lineGenerator = default;
    [SerializeField] float initRadius = 300f;

    private DragonCurve dragonCurve = null;
    private Dictionary<int, Vector3[]> curveTable = new Dictionary<int, Vector3[]>();
    private Dictionary<int, Vector3[]> lineTable = new Dictionary<int, Vector3[]>();
    private Dictionary<int, float> widthTable = new Dictionary<int, float>();

    private int currentIteration = 1;
    public readonly int kMaxIteration = 15;

    // Start is called before the first frame update
    void Start()
    {
        buttonSplit.onClick.AddListener(() =>
        {
            if (currentIteration >= kMaxIteration || lineGenerator.IsInFade) return;
            currentIteration++;
            UpdateIterationLabel();
            lineGenerator.UpdateLine(lineTable[currentIteration], curveTable[currentIteration], widthTable[currentIteration]);
        });
        buttonMerge.onClick.AddListener(() =>
        {
            if (currentIteration <= 1 || lineGenerator.IsInFade) return;
            lineGenerator.UpdateLine(curveTable[currentIteration], lineTable[currentIteration], widthTable[currentIteration]);
            currentIteration--;
            UpdateIterationLabel();
        });

        dragonCurve = new DragonCurve(false);

        // pre calculate curves for all iteration
        List<Vector3> curve = new List<Vector3>();
        curve.Add(new Vector3(-initRadius, initRadius / 2, 0));
        curve.Add(new Vector3(0, -initRadius / 2, 0));
        curve.Add(new Vector3(initRadius, initRadius / 2, 0));
        lineTable[1] = curve.ToArray();
        curveTable[1] = curve.ToArray();

        float lineWidth = 0.5f;
        widthTable[1] = lineWidth;

        for (int i = 2; i <= kMaxIteration; i++)
        {
            List<Vector3> splitLine = dragonCurve.SplitByLine(curve);
            List<Vector3> splitCurve = dragonCurve.SplitByCurve(curve);
            lineTable[i] = splitLine.ToArray();
            curveTable[i] = splitCurve.ToArray();

            lineWidth = lineWidth / 1.4f;
            widthTable[i] = lineWidth;

            curve = splitCurve;
        }

        currentIteration = 1;
        UpdateIterationLabel();
        lineGenerator.UpdateLine(curveTable[currentIteration], widthTable[currentIteration]);
    }

    void UpdateIterationLabel()
    {
        labelIteration.text = "Iteration : " + currentIteration.ToString();
    }
}
