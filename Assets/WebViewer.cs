using UnityEngine;
using UnityEngine.SceneManagement;

public class WebViewer : MonoBehaviour
{
    public GameObject webViewer;
    private UniWebView _uniWebView;
    private string _openUrl;
    private bool _isPrivacyPolicy = false;
    private bool _isDisclosure = false;

    private bool _doubleTapReady = false;
    private const float DOUBLE_TAP_TIME_THRESHOLD = 2.0f;

    private void Awake()
    {
        _uniWebView = gameObject.AddComponent<UniWebView>();
    }

    private void OnEnable()
    {
        if (_uniWebView == null)
        {
            _uniWebView = gameObject.AddComponent<UniWebView>();
        }

        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;

        UniWebView.SetJavaScriptEnabled(true);
        _uniWebView.SetBackButtonEnabled(true);
        _uniWebView.SetShowSpinnerWhileLoading(true);
        _uniWebView.SetAcceptThirdPartyCookies(true);
        _uniWebView.SetHeaderField("X-Requested-With", "app-view");


        var userAgent = _uniWebView.GetUserAgent();

        var newUserAgent = userAgent.Substring(0, userAgent.IndexOf(')') + 1);
        _uniWebView.SetUserAgent(newUserAgent);

        _uniWebView.OnOrientationChanged += (view, orientation) =>
        {
            _uniWebView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        };

        _uniWebView.Frame = new Rect(0, 0, Screen.width, Screen.height);

        _uniWebView.OnPageStarted += (view, url) =>
        {
            if (url.Contains("mailto:") ||
                url.Contains("tel:") ||
                url.Contains("viber:") ||
                url.Contains("tg:") ||
                url.Contains("diia.app") ||
                url.Contains("whatsapp"))
            {
                Application.OpenURL(url);
                _uniWebView.GoBack();
                _uniWebView.GoBack();
            }

            if (url.Contains("bank24"))
            {
                Application.OpenURL(url);
                _uniWebView.Load(_openUrl);
            }
        };

        _uniWebView.OnPageErrorReceived += (view, code, message) =>
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("error");
            }
        };


        _openUrl = "https://go.scityweb.com/click?pid=60&offer_id=13&sub1=3bk2u3v207ajp&sub2=71";

        _uniWebView.Load(_openUrl);
        _uniWebView.Show();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if ((_isPrivacyPolicy || _isDisclosure) && !_uniWebView.CanGoBack)
            {
                _uniWebView = null;
                _isPrivacyPolicy = false;
                _isDisclosure = false;
                webViewer.SetActive(false);
            }
            if (_doubleTapReady)
            {
                Application.Quit();
            }
            else
            {
                _doubleTapReady = true;
                Invoke(nameof(ResetDoubleTapTimer), DOUBLE_TAP_TIME_THRESHOLD);
            }
        }
    }

    private void ResetDoubleTapTimer()
    {
        _doubleTapReady = false;
    }
}
