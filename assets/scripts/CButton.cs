using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CButton : MonoBehaviour 
{
    public int _iLien;
    public int _iBudget;
    public int _iQualite;

	public int _iPrecarity;
	public int _iFineAtHome;
	public int _iDisturbedEnvironment;

    private MAppManager _appManager;
    private CMainManager _mainManager;

    void Start()
    {
        _appManager = MAppManager.Get();
    }


	public void OnClick()
    {
        _mainManager = CMainManager.Get();
		CGoal oGoal = _mainManager.GetCurrentGoalScript();

		//Early out: do not increment scores again
		if ( oGoal.IsSelectionDone( ) )
		{
			return;
		}

		oGoal.SelectionDone( true );

		// Update panel scores
		int iOldBudgetScore = _appManager.GetBudgetScore();
		int iOldLienScore = _appManager.GetLienScore();
		int iOldQualite = _appManager.GetQualiteScore();
        _appManager.SetBudgetScore(iOldBudgetScore + _iBudget);
        _appManager.SetLienScore(iOldLienScore + _iLien);
        _appManager.SetQualiteScore(iOldQualite + _iQualite);

		// Update End scores
		int iOldPrecarityScore = _appManager.GetPrecarityScore();
		int iOldFineAtHomeScore = _appManager.GetFineAtHomeScore();
		int iOldDisturbedEnvironment = _appManager.GetDisturbedEnvironmentScore();
		_appManager.SetPrecarityScore( iOldPrecarityScore + _iPrecarity );
		_appManager.SetFineAtHomeScore( iOldFineAtHomeScore + _iFineAtHome );
		_appManager.SetDisturbedEnvironmentScore( _iDisturbedEnvironment + _iDisturbedEnvironment );

        Debug.Log("Current Score:" + _appManager.GetLienScore() + " " + _appManager.GetBudgetScore() + " " + _appManager.GetQualiteScore());



		_mainManager.UpdateScore( );


		if (_mainManager.GetCurrentGoal() == 1 || _mainManager.GetCurrentGoal() == 5 || _mainManager.GetCurrentGoal() == 8 || _mainManager.GetCurrentGoal() == 3)
        {
            if(this.gameObject.transform.parent.name == "panel_left")
            {
                _mainManager.ResponseBeforeNextGoal(1);
            }
            else if (this.gameObject.transform.parent.name == "panel_right")
            {
                _mainManager.ResponseBeforeNextGoal(0);
            }
            
        }

//        else
//        {
//            _mainManager.NextGoal();
//        }
//        
        
    }
}
