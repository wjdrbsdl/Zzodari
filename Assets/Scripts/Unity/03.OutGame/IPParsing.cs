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
    public List<int[]> MatchCode; //enum�� ��ġ�Ǵ� �ε���
    public List<string[]> DbValueList; //��񿡼� ���� ����

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
    //stat[] �� ����ϴ°�� db�� enum�� MatchValue�� ����� ���� � enum�� ������ 
    private System.Enum[] matchTypes = { null};
    private Dictionary<EMasterData, ParseData> dbContainer = new(); //�Ľ��Ѱ��� �׳� ���� �ִ»��� - ����ϴ°����� �ٽ� ���� �ʿ�. 

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

                //������ ������ �Ľ��ؼ� ���
                IPAddress ipAddress = IPAddress.Parse(parseData.DbValueList[0][1]);
                FixedValue.ServerIp = ipAddress;
            }
            if (parseData.DbValueList.Count >= 2)
            {
                //TestAgora._token = parseData.DbValueList[1][1];
            }
            if (parseData.DbValueList.Count >= 3)
            {
                //�ð� Ÿ�̸�
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
             //�Ľ� ���� �� �� ��
         },
         ClassfyDataBase)
            );
    }

    private void ClassfyDataBase(bool _successLoad, int _index, string message)
    {
        System.Enum parseEnum = matchTypes[_index];
        //��� �Ŵ������� Ŭ������ ������ֵ��� ������ �з�
        if (_successLoad)
        {
            //1. �Ľ��� Ÿ�� - �������� �������� ��Ī�صа�docuID - parseTypes
            EMasterData parseData = dbId[_index];
            //2. sheetData �ึ�� �и�
            string[] enterDivde = message.Split('\n'); //���� - �� �и�
            //3. ù��° ���� enum string�� �� db�� ������ ����� Į�������� ������� �κ�
            string[] dbEnumCode = enterDivde[0].Trim().Split('\t'); //enumCode�� Į�������� - ù��° ���� ����
            //4. sheet ���� ���� enum���� �ش� Į������ �ε����� �ٸ� �� �����Ƿ� ����
            List<int[]> matchCode = MakeMatchCode(parseEnum, dbEnumCode);
            //5. ������ ���� ���� ����.
            List<string[]> dbValueList = new();
            for (int i = 1; i < enterDivde.Length; i++) //1����� �ڷ� ��
            {
                if (enterDivde[i][0].Equals('#'))
                {
                    //   Debug.Log(enterDivde[i] + "ù���� #���� �����ϴ� ���� �ǳʶ�");
                    continue;
                }


                string[] valueDivde = enterDivde[i].Trim().Split('\t'); //�� - �� �и� 

              //  Debug.Log(parseData + "�� ������" + valueDivde.Length);
                dbValueList.Add(valueDivde);
            }
            //6. �Ľ��ڵ忡 - �ε��� ��Ī �ڵ�� ���� ������ struct�� ��� dctionary�� ���� 
            dbContainer.Add(parseData, new ParseData(matchCode, dbValueList));

        }
        else
            Debug.LogWarning(matchTypes[_index] + "�Ľ� ����");
    }

    public IEnumerator GetSheetDataCo(string documentID, string[] sheetID, Action doneAct = null,
   Action<bool, int, string> process = null)
    {
        int doneWork = sheetID.Length; //ó���ؾ��� ����
        int curWork = 0;//ó���� ����
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

        doneAct?.Invoke(); //������ GameManager�� �۾� �Ϸ������� �˸�. 
    }

    public static List<int[]> MakeMatchCode(System.Enum _codeEnum, string[] _dbCodes)
    {
        //db�� ����Code ���� �ΰ����� enumCode ���� ��Ī
        //�����ڵ��� 0��°�� enumCode�� �� ��°���� ¥�� ��ȯ
        List<int[]> matchCodeList = new();

        if (_codeEnum == null)
            return matchCodeList;

        string[] enumCodes = ParseEnumStrings(_codeEnum);
        //���� Ǯ ��ġ �������ϳ�
        for (int dbIndex = 0; dbIndex < _dbCodes.Length; dbIndex++)
        {
            string dbCode = _dbCodes[dbIndex];
            for (int enumIndex = 0; enumIndex < enumCodes.Length; enumIndex++)
            {
                string enumCode = enumCodes[enumIndex];
                if (dbCode == enumCode)
                {
                    int[] match = { dbIndex, enumIndex }; //��� x��°�� ���� �̳�y��° ���ΰɷ� �ڵ� ����
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
