using UnityEngine;

namespace Assets.Scripts
{
    public class ReachBack : MonoBehaviour
    {
        public ThrowUI throwUI;
        //  public GameObject reachBackLines;
        public GameObject powerBackground;
        public ReachBackDisc reachBackDisc;
        public Quaternion startRot;
        public Quaternion targetRot;
        public Quaternion dampingRot;
        public float changeRot = 0f;
        public bool changeAccuracy = false;

        public void ShowReachBackPanel(bool show)
        {
            powerBackground.SetActive(false);
            gameObject.SetActive(show);
            reachBackDisc.lr = throwUI.playerScript.reachBackLineRenderer;
        }

        internal void BeginReachBack()
        {
            powerBackground.SetActive(true);
            throwUI.playerScript.throwUI.HideThrowUI();
            if (Menu.PlayerSave.playerGet)
            {
                if (!throwUI.playerScript.gameState.playerSave.GetTutorialCheck().seenFirstThrow)
                {
                    throwUI.playerScript.UI.Tutorial().DuringThrow();
                }
            }
        }

        internal void CancelReachback()
        {
            powerBackground.SetActive(false);
            HyzerAngleChange(0);
            throwUI.playerScript.throwUI.ShowThrowUI();
            throwUI.playerScript.UI.Tutorial().ThrowComplete();
            throwUI.playerScript.transform.rotation = startRot;
        }

        public void HyzerAngleChange(float value)
        {
            var rot = throwUI.playerScript.curentDisc.transform.localRotation;
            throwUI.playerScript.curentDisc.transform.localRotation = Quaternion.Euler(rot.eulerAngles.x, 0, value * -1);
        }

        public void NoseAngleChange(float value)
        {
            var rot = throwUI.playerScript.curentDisc.transform.localRotation;
            throwUI.playerScript.curentDisc.transform.localRotation = Quaternion.Euler(rot.eulerAngles.x, 0, value);
        }

        internal void AccuracyChange()
        {
            if (!changeAccuracy) return;
            if (changeRot > 1f)
            {
                dampingRot = targetRot;
                targetRot = startRot * Quaternion.AngleAxis(1f, Random.insideUnitCircle);
                //targetRot = throwUI.playerScript.transform.rotation * Quaternion.AngleAxis(Random.Range(-10f, 10f), Vector3.up);
                changeRot = 0.1f;
            }

            var t = Quaternion.Slerp(dampingRot, targetRot, changeRot);
            throwUI.playerScript.transform.rotation = Quaternion.Slerp(throwUI.playerScript.transform.rotation, t, 2f* Time.deltaTime);
            changeRot += Random.Range(1f, 2f)*Time.deltaTime;
        }
    }
}