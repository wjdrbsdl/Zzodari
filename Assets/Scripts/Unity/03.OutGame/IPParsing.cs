using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public enum EMasterData
{
    PublicIP
}
public struct ParseData
{
    public List<int[]> MatchCode; //enum과 매치되는 인덱스
    public List<string[]> DbValueList; //디비에서 따온 값들

    public ParseData(List<int[]> _matchCode, List<string[]> _dbValues)
    {
        MatchCode = _matchCode;
        DbValueList = _dbValues;
    }

    public void NullCheck()
    {
        if (MatchCode == null)
        {
            MatchCode = new();
            DbValueList = new();
        }
    }
}

public class IPParsing : MonoBehaviour
{
    private static string docuIDes = "1LOA-0bDhVRHGuggJgU6oa-F3LMmyLJfUm7S8vLyXwos";
    private string[] sheetIDes = { "0"};
    private EMasterData[] dbId = { EMasterData.PublicIP};
    //stat[] 를 사용하는경우 db에 enum값 MatchValue를 만들기 위해 어떤 enum을 쓰는지 
    private System.Enum[] matchTypes = { null};
    private Dictionary<EMasterData, ParseData> dbContainer = new(); //파싱한값을 그냥 갖고만 있는상태 - 사용하는곳에서 다시 가공 필요. 

    public void Awake()
    {
        if(FixedValue.ServerIp == null)
        ParseSheetData();
    }

    public void Start()
    {
        if (FixedValue.ServerIp == null)
            StartCoroutine(ParsingPublicIP());
    }

    private IEnumerator ParsingPublicIP()
    {
        bool isFind = false;
        bool haveTime = true;
        float limitTime = 4f;
        while(isFind == false && haveTime)
        {
            limitTime -= Time.deltaTime;
            ParseData parseData = GetMasterData(EMasterData.PublicIP);
            for (int i = 0; i < parseData.DbValueList.Count; i++)
            {
                isFind = true;
               // Debug.Log(parseData.DbValueList[i][0] + " "+ parseData.DbValueList[i][1]);
            }

            if(parseData.DbValueList.Count>=1)
            {
               // InputManager.instance.SetInputTest(parseData.DbValueList[0][1]);

                //접속할 아이피 파싱해서 등록
                IPAddress ipAddress = IPAddress.Parse(parseData.DbValueList[0][1]);
                FixedValue.ServerIp = ipAddress;
            }
            if (parseData.DbValueList.Count >= 2)
            {
                //TestAgora._token = parseData.DbValueList[1][1];
            }
            if (parseData.DbValueList.Count >= 3)
            {
                //시간 타이머
                UserTurnTimer.TurnTime = int.Parse(parseData.DbValueList[2][1]);
            }

            if (limitTime < 0)
            {
                haveTime = false;
            }
            yield return null;
        }
    }


    public ParseData GetMasterData(EMasterData _dataId)
    {
        dbContainer.TryGetValue(_dataId, out ParseData _parseData);
        _parseData.NullCheck();
        return _parseData;
    }

    private void ParseSheetData()
    {
        StartCoroutine(
            GetSheetDataCo(docuIDes, sheetIDes,
         delegate {
             //파싱 성공 후 할 것
         },
         ClassfyDataBase)
            );
    }

    private void ClassfyDataBase(bool _successLoad, int _index, string message)
    {
        System.Enum parseEnum = matchTypes[_index];
        //담당 매니저에서 클래스를 만들수있도록 데이터 분류
        if (_successLoad)
        {
            //1. 파싱한 타입 - 수동으로 변수에서 매칭해둔거docuID - parseTypes
            EMasterData parseData = dbId[_index];
            //2. sheetData 행마다 분리
            string[] enterDivde = message.Split('\n'); //엔터 - 행 분리
            //3. 첫번째 행은 enum string값 중 db로 관리할 목록을 칼럼명으로 적어놓은 부분
            string[] dbEnumCode = enterDivde[0].Trim().Split('\t'); //enumCode를 칼럼명으로 - 첫번째 행의 역할
            //4. sheet 열과 현재 enum에서 해당 칼럼명의 인덱스가 다를 수 있으므로 조정
            List<int[]> matchCode = MakeMatchCode(parseEnum, dbEnumCode);
            //5. 나머지 행은 실제 값들.
            List<string[]> dbValueList = new();
            for (int i = 1; i < enterDivde.Length; i++) //1행부터 자료 값
            {
                if (enterDivde[i][0].Equals('#'))
                {
                    //   Debug.Log(enterDivde[i] + "첫열이 #으로 시작하는 행은 건너띔");
                    continue;
                }


                string[] valueDivde = enterDivde[i].Trim().Split('\t'); //탭 - 열 분리 

              //  Debug.Log(parseData + "행 사이즈" + valueDivde.Length);
                dbValueList.Add(valueDivde);
            }
            //6. 파싱코드에 - 인덱스 매칭 코드와 실제 값들을 struct로 묶어서 dctionary에 저장 
            dbContainer.Add(parseData, new ParseData(matchCode, dbValueList));

        }
        else
            Debug.LogWarning(matchTypes[_index] + "파싱 실패");
    }

    public IEnumerator GetSheetDataCo(string documentID, string[] sheetID, Action doneAct = null,
   Action<bool, int, string> process = null)
    {
        int doneWork = sheetID.Length; //처리해야할 숫자
        int curWork = 0;//처리한 숫자
        while (curWork < doneWork)
        {
            string url = $"https://docs.google.com/spreadsheets/d/{documentID}/export?format=tsv&gid={sheetID[curWork]}";

            UnityWebRequest req = UnityWebRequest.Get(url);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.responseCode != 200)
            {

                process?.Invoke(false, curWork, null);
            }
            else
            {
                process?.Invoke(true, curWork, req.downloadHandler.text);
            }

            curWork += 1;
        }

        doneAct?.Invoke(); //보통은 GameManager에 작업 완료했음을 알림. 
    }

    public static List<int[]> MakeMatchCode(System.Enum _codeEnum, string[] _dbCodes)
    {
        //db의 벨류Code 값과 인게임의 enumCode 값을 매칭
        //벨류코드의 0번째가 enumCode의 몇 번째인지 짜서 반환
        List<int[]> matchCodeList = new();

        if (_codeEnum == null)
            return matchCodeList;

        string[] enumCodes = ParseEnumStrings(_codeEnum);
        //거의 풀 매치 돌려야하네
        for (int dbIndex = 0; dbIndex < _dbCodes.Length; dbIndex++)
        {
            string dbCode = _dbCodes[dbIndex];
            for (int enumIndex = 0; enumIndex < enumCodes.Length; enumIndex++)
            {
                string enumCode = enumCodes[enumIndex];
                if (dbCode == enumCode)
                {
                    int[] match = { dbIndex, enumIndex }; //디비 x번째의 값이 이넘y번째 값인걸로 코드 산출
                    matchCodeList.Add(match);
                    break;
                }
            }
        }
        return matchCodeList;
    }

    public static string[] ParseEnumStrings(System.Enum _enumValue)
    {
        return System.Enum.GetNames(_enumValue.GetType());
    }

}
