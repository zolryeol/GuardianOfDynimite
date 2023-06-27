using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
public class LeaderBoard : MonoBehaviour
{
    private static LeaderBoard instance = null;

    public static LeaderBoard Instance
    {
        get
        {
            if (instance == null)
            {
                return new GameObject("LeaderBoard").AddComponent<LeaderBoard>();
            }
            else return instance;
        }
    }

    //     private Text rankingText;
    // 
    //     Text[] Rank = new Text[10];
    //     Text[] Name = new Text[10];
    //     Text[] Score = new Text[10];

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);


        //         rankingText = this.gameObject.GetComponent<Text>();
        //         for (int i = 0; i < 10; ++i)
        //         {
        //             //Rank[i] = this.gameObject.transform.Find("Rank").gameObject.transform.GetChild(i).GetComponent<Text>();
        //             Name[i] = this.gameObject.transform.Find("Name").gameObject.transform.GetChild(i).GetComponent<Text>();
        //             Score[i] = this.gameObject.transform.Find("Score").gameObject.transform.GetChild(i).GetComponent<Text>();
        //         }

    }
    public void Start()
    {
        if (SteamManager.Initialized)
        {
            Debug.Log($"Steam의 초기화 성공, AppID : {SteamUtils.GetAppID()}");
            FindLeaderBoard();
        }
        else
        {
            Debug.Log("Steam초기화 실패");
        }
    }

    //     public void Start()
    //     {
    //         if (SteamManager.Initialized)
    //         {
    //             Debug.Log($"Steam의 초기화 성공, AppID : {SteamUtils.GetAppID()}");
    //         }
    //         else
    //         {
    //             Debug.Log("Steam초기화 실패");
    //         }
    //         //FindLeaderBoard();
    //     }

    /// 리더보드 가져오기
    /// 
    public void FindLeaderBoard()
    {
        Debug.Log("리더보드를 검색한다");

        CallResult<LeaderboardFindResult_t>.Create().Set(SteamUserStats.FindLeaderboard("RankingForTest"), OnFindLeaderboard);

        //CallResult<LeaderboardFindResult_t>.Create().Set(SteamUserStats.FindLeaderboard("RankingForTest"), UploadScore);

        CallResult<LeaderboardFindResult_t>.Create().Set(SteamUserStats.FindLeaderboard("RankingForTest"), DownloadEntries);
    }

    public void GetLeaderBoardInfo()
    {
        CallResult<LeaderboardFindResult_t>.Create().Set(SteamUserStats.FindLeaderboard("RankingForTest"), DownloadEntries);
    }

    public void SendTryCount()
    {
        Debug.Log("샌드 트라이 카운트 호출 + 프리트라이카운트" + PlayerPrefs.GetInt("PreTryCount"));

        if (PlayerPrefs.GetInt("TryCount") <= PlayerPrefs.GetInt("PreTryCount", 20000))
        {
            Debug.Log("프리 카운트보다 작으니까 호출합니다." + "프리트라이카운트" + PlayerPrefs.GetInt("PreTryCount"));

            PlayerPrefs.SetInt("PreTryCount", PlayerPrefs.GetInt("TryCount"));

            CallResult<LeaderboardFindResult_t>.Create().Set(SteamUserStats.FindLeaderboard("RankingForTest"), UploadScore); // 갱신시킨다.
        }
        else
        {
            PlayerPrefs.SetInt("TryCount", 0);
            Debug.Log("기록갱신안되지만 초기화" + PlayerPrefs.GetInt("TryCount"));
        }

    }

    void OnFindLeaderboard(LeaderboardFindResult_t _result, bool _failure)
    {
        if (_failure || _result.m_bLeaderboardFound == 0)
        {
            Debug.Log("리더보드 를 찾을 수 없다");
            return;
        }

        var leaderboard = _result.m_hSteamLeaderboard;
        Debug.Log($"리더보드 이름 : { SteamUserStats.GetLeaderboardName(leaderboard)}");
        Debug.Log($"리더보드에 등록되어있는 수 : { SteamUserStats.GetLeaderboardEntryCount(leaderboard)}");
        Debug.Log($"리더보드 소트방법 : { SteamUserStats.GetLeaderboardSortMethod(leaderboard)}");
        Debug.Log($"리더보드 표시타입 : { SteamUserStats.GetLeaderboardDisplayType(leaderboard)}");
    }

    /// 스코어 송신

    private void UploadScore(LeaderboardFindResult_t _result, bool _failure)
    {
        if (_failure || _result.m_bLeaderboardFound == 0)
        {
            Debug.LogWarning("리더보드를 찾을 수 없다");
            return;
        }

        int _tryCount = PlayerPrefs.GetInt("TryCount");
        Debug.Log("트라이카운트 = " + _tryCount);

        var call = SteamUserStats.UploadLeaderboardScore(_result.m_hSteamLeaderboard, // 보낼 리더보드
            ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, // 보내는 스코어가 best를 넘을면 갱신한다
            _tryCount, new int[0], 0);
        Debug.Log($"스코어 송신 시작");

        CallResult<LeaderboardScoreUploaded_t>.Create().Set(call, OnUpLoadScore);

    }

    private void OnUpLoadScore(LeaderboardScoreUploaded_t _result, bool _failure)
    {
        Debug.Log("트라이카운트 초기화구문");
        PlayerPrefs.SetInt("TryCount", 0);


        if (_failure || _result.m_bSuccess == 0)
        {
            Debug.LogWarning("스코어 송신에 실패했다");
            return;
        }
        Debug.Log("스코어 송신완료");
        //갱신결과 확인
        Debug.Log($"현재 스코어 : {_result.m_nScore}");


        Debug.Log($"스코어가 갱신되었는가? : {_result.m_bScoreChanged}");
        Debug.Log($"스코어 보내기전 순위 : {_result.m_nGlobalRankPrevious}");
        Debug.Log($"스코어 보낸후 순위 : {_result.m_nGlobalRankNew}");
    }

    /// 순위 가져오기
    private void DownloadEntries(LeaderboardFindResult_t _result, bool _failure)
    {
        //리더보드를 찾았는가 판정
        if (_failure || _result.m_bLeaderboardFound == 0)
        {
            Debug.LogWarning("리더보드를 찾지 못했다.");
            return;
        }

        // 가져온 정보
        var call = SteamUserStats.DownloadLeaderboardEntries(
          _result.m_hSteamLeaderboard, //보낼 리더보드 
          ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, //순위 가져올 범위
          0, //가져올 순위의 가장위
          100 //가져올 순위의 가장아래

        /// 앞뒤로 -2위 +2위까지 찾는것
        //         var call = SteamUserStats.DownloadLeaderboardEntries(
        //           _result.m_hSteamLeaderboard, //보낼 리더보드 
        //           ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, //순위 가져올 범위
        //           -2, //가져올 순위의 가장위
        //           2 //가져올 순위의 가장아래

        );

        // 순위 취득용 송신
        Debug.Log("순위가져오기 시작");
        CallResult<LeaderboardScoresDownloaded_t>.Create().Set(call, OnDownloadEntriese);
    }

    /// 순위가져오기 완료
    private void OnDownloadEntriese(LeaderboardScoresDownloaded_t result, bool failure)
    {
        if (SceneMG.Instance.GetNowScene() != 0)
        {
            UIManager.Instance.RankingTextColorReset();

            // 제대로 가져왔는가
            if (failure)
            {
                Debug.LogWarning("순위표를 가져오는데 실패했다.");
                return;
            }
            Debug.Log($"순위표 가져오기 완료");

            //登録されている順位の個数を確認 등록되어있는 순위의 개수를 확인한다
            Debug.Log($"가져온 순위의 개수 : {result.m_cEntryCount}");

            int j = 0;

            int preRank = 0;
            int preScore = 0;

            //各順位の情報を確認 // 각 순위의 정보를 확인한다.
            for (int i = UIManager.Instance.rankingPageIndex * 5; i < (UIManager.Instance.rankingPageIndex * 5) + 5/*result.m_cEntryCount*/; i++)
            {
                //各順位の情報を取得 // 각순위의 정보를 가져온다.
                LeaderboardEntry_t leaderboardEntry;
                var success = SteamUserStats.GetDownloadedLeaderboardEntry(result.m_hSteamLeaderboardEntries, i, out leaderboardEntry, new int[0], 0);
                //rankingText.text += $"Rank : {leaderboardEntry.m_nGlobalRank } " + $" Name : {SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser)}" + $" score : {leaderboardEntry.m_nScore} \n";

                //             情報を表示 정보를 표시
                //             Debug.Log($"순위 : {leaderboardEntry.m_nGlobalRank}");
                //             Debug.Log($"ID : {leaderboardEntry.m_steamIDUser}");
                //             Debug.Log($" 점수 : {leaderboardEntry.m_nScore}");
                //             Debug.Log($" 유저이름 : {SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser)}");

                if (success)
                {
                    UIManager.Instance.RankTexts[j * 3].text = (i + 1).ToString();         // 순위



                    UIManager.Instance.RankTexts[(j * 3) + 1].text = SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser);     // Name

                    if (leaderboardEntry.m_steamIDUser == SteamUser.GetSteamID())
                    {
                        UIManager.Instance.RankTexts[(j * 3) + 1].color = Color.yellow;
                    }

                    UIManager.Instance.RankTexts[(j * 3) + 2].text = leaderboardEntry.m_nScore.ToString();     // Count

                    /// 공동순위 예외처리?
                    if (preScore == int.Parse(leaderboardEntry.m_nScore.ToString())) // 만약에 내 윗순위와 점수가 같다면 순위표시를 공동으로한다.
                    {
                        UIManager.Instance.RankTexts[j * 3].text = preRank.ToString();         // 순위
                    }

                    preRank = int.Parse(UIManager.Instance.RankTexts[j * 3].text);
                    preScore = int.Parse(UIManager.Instance.RankTexts[(j * 3) + 2].text);
                }
                else
                {
                    UIManager.Instance.RankTexts[j * 3].text = (i + 1).ToString();         // 순위
                    UIManager.Instance.RankTexts[(j * 3) + 1].text = "None";
                    UIManager.Instance.RankTexts[(j * 3) + 2].text = "None";
                    //return;
                }

                j++;

            }
        }
    }

    private void DownloadEntries(LeaderboardFindResult_t _result, bool _failure, List<GameObject> l)
    {
        if (_failure || _result.m_bLeaderboardFound == 0)
        {
            Debug.LogWarning("리더보드를 찾지 못했다.");
            return;
        }

        // 가져온 정보
        var call = SteamUserStats.DownloadLeaderboardEntries(
          _result.m_hSteamLeaderboard, //보낼 리더보드 
          ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, //순위 가져올 범위
          0, //가져올 순위의 가장위
          100 //가져올 순위의 가장아래
        );

        // 순위 취득용 송신
        Debug.Log("순위가져오기 시작");
        CallResult<LeaderboardScoresDownloaded_t>.Create().Set(call, OnDownloadEntriese);
    }

}



//     private void MyEntry(LeaderboardFindResult_t _result, bool _failure)
//     {
//         //리더보드를 찾았는가 판정
//         if (_failure || _result.m_bLeaderboardFound == 0)
//         {
//             Debug.LogWarning("MyEntry함수에서 리더보드를 찾지 못했다.");
//             return;
//         }
// 
//         CSteamID[] targetIDs = { SteamUser.GetSteamID() };//取得したい情報のIDに、自分のIDを設定
//         var call = SteamUserStats.DownloadLeaderboardEntriesForUsers(
//           _result.m_hSteamLeaderboard, //送信するリーダーボード 
//           targetIDs,       //取得したいUserのID
//           targetIDs.Length //取得したい情報の個数 
//           );
// 
//         CallResult<LeaderboardScoresDownloaded_t>.Create().Set(call, OnDownloadEntriese);
//     }

