using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlacksiteGame : MonoBehaviour
{
    private readonly HashSet<string> found = new HashSet<string>();
    private Text status;
    private Text detail;
    private Font font;

    private void Start()
    {
        Application.targetFrameRate = 60;
        BuildRoom();
        BuildUI();
        Show("ДЕЛО 07 / ЧЁРНЫЙ СИГНАЛ\n\n03:19. Станция наблюдения отключена от внешней сети. Изучите три материала, найдите противоречие и установите, кто подделал отчёт.");
    }

    private Material Mat(Color c, float metallic = 0f)
    {
        var m = new Material(Shader.Find("Standard"));
        m.color = c;
        m.SetFloat("_Metallic", metallic);
        m.SetFloat("_Glossiness", 0.42f);
        return m;
    }

    private GameObject Cube(string n, Vector3 p, Vector3 s, Color c, float metal = 0f)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = n; g.transform.position = p; g.transform.localScale = s;
        g.GetComponent<Renderer>().material = Mat(c, metal);
        return g;
    }

    private void BuildRoom()
    {
        RenderSettings.ambientLight = new Color(.18f, .23f, .25f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(.012f, .02f, .026f);
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = .025f;
        Cube("Floor", new Vector3(0,-.15f,0), new Vector3(12,.2f,12), new Color(.035f,.055f,.065f), .25f);
        Cube("Back wall", new Vector3(0,1.85f,-4), new Vector3(12,4,.2f), new Color(.045f,.07f,.08f));
        Cube("Left wall", new Vector3(-6,1.85f,0), new Vector3(.2f,4,8), new Color(.025f,.045f,.055f));
        Cube("Right wall", new Vector3(6,1.85f,0), new Vector3(.2f,4,8), new Color(.025f,.045f,.055f));
        Cube("Desk", new Vector3(0,.72f,-1.8f), new Vector3(4.2f,.18f,1.6f), new Color(.09f,.12f,.13f), .45f);
        Cube("Monitor", new Vector3(0,1.38f,-2.35f), new Vector3(1.8f,1.05f,.12f), new Color(.015f,.04f,.05f), .2f);
        Cube("Monitor glow", new Vector3(0,1.39f,-2.275f), new Vector3(1.62f,.85f,.025f), new Color(.025f,.28f,.31f));
        for (int i=0;i<5;i++) {
            Cube("Server rack "+i, new Vector3(-4.6f,.28f+i*.46f,-2.9f), new Vector3(.95f,.42f,.62f), new Color(.045f,.075f,.085f), .3f);
            Cube("Warning LED "+i, new Vector3(-4.35f,.34f+i*.46f,-2.57f), new Vector3(.08f,.045f,.025f), new Color(.9f,.12f,.12f));
        }
        Cube("Evidence server log", new Vector3(-1.4f,.84f,-2), new Vector3(.22f,.06f,.16f), new Color(.2f,.85f,.9f), .25f);
        Cube("Evidence photo", new Vector3(1.2f,.79f,-2.3f), new Vector3(.22f,.06f,.16f), new Color(.9f,.55f,.18f), .25f);
        Cube("Evidence memo", new Vector3(0,.94f,-1), new Vector3(.22f,.06f,.16f), new Color(.95f,.22f,.28f), .25f);
        var light = new GameObject("Cold overhead light").AddComponent<Light>();
        light.type = LightType.Directional; light.color = new Color(.42f,.62f,.7f); light.intensity = 1.15f;
        light.transform.rotation = Quaternion.Euler(48,25,0);
        var cam = new GameObject("Main Camera").AddComponent<Camera>();
        cam.tag = "MainCamera"; cam.transform.position = new Vector3(0,2.2f,5.7f);
        cam.transform.rotation = Quaternion.Euler(12,0,0); cam.fieldOfView = 62;
        cam.backgroundColor = new Color(.012f,.02f,.03f);
    }

    private void BuildUI()
    {
        var canvasGO = new GameObject("Case UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1280,720);
        font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        var header = Panel(canvas.transform, "Header", new Vector2(0,1), new Vector2(1,1), new Vector2(0,-64), new Vector2(0,0), new Color(.015f,.035f,.045f,.96f));
        Label(header, "BLACKSITE  /  OPERATOR", 24, new Vector2(20,-12), new Vector2(650,-52), new Color(.45f,.92f,.94f), TextAnchor.MiddleLeft);
        Label(header, "CASE 07  •  LOCAL MODE", 16, new Vector2(-340,-12), new Vector2(-20,-52), new Color(.9f,.42f,.3f), TextAnchor.MiddleRight);
        var bottom = Panel(canvas.transform,"Evidence Console",new Vector2(0,0),new Vector2(1,0),new Vector2(0,0),new Vector2(0,250),new Color(.012f,.025f,.033f,.97f));
        Label(bottom,"EVIDENCE CONSOLE",18,new Vector2(18,-8),new Vector2(360,-42),new Color(.45f,.92f,.94f),TextAnchor.MiddleLeft);
        status = Label(bottom,"FOUND: 0 / 3",16,new Vector2(-250,-8),new Vector2(-18,-42),new Color(.9f,.65f,.3f),TextAnchor.MiddleRight);
        detail = Label(bottom,"Выберите материал для анализа.",17,new Vector2(22,-52),new Vector2(-22,-130),new Color(.78f,.85f,.86f),TextAnchor.UpperLeft);
        Button(bottom,"СЕРВЕРНЫЙ ЖУРНАЛ",new Vector2(18,-142),new Vector2(300,-190),new Color(.04f,.17f,.19f),()=>Inspect("server","СЕРВЕРНЫЙ ЖУРНАЛ","03:17:42 — архив открыт внутренним ключом. Подпись принадлежит сотруднику с позывным MOTH."));
        Button(bottom,"ФОТОГРАФИЯ",new Vector2(320,-142),new Vector2(600,-190),new Color(.19f,.12f,.05f),()=>Inspect("photo","ФОТОГРАФИЯ С МЕСТА","В отражении стекла виден второй человек. В официальном отчёте он не упомянут."));
        Button(bottom,"ЗАПИСКА",new Vector2(620,-142),new Vector2(900,-190),new Color(.19f,.05f,.06f),()=>Inspect("memo","ЗАПИСКА ОПЕРАТИВА","«Если увидишь красный свет — не доверяй временным меткам. Они переписаны»."));
        Button(bottom,"СВЕРИТЬ УЛИКИ  →",new Vector2(920,-142),new Vector2(1238,-190),new Color(.22f,.045f,.04f),Solve);
    }

    private RectTransform Panel(Transform parent,string n,Vector2 amin,Vector2 amax,Vector2 omin,Vector2 omax,Color color)
    {
        var go=new GameObject(n,typeof(RectTransform),typeof(Image)); go.transform.SetParent(parent,false);
        var r=go.GetComponent<RectTransform>(); r.anchorMin=amin;r.anchorMax=amax;r.offsetMin=omin;r.offsetMax=omax;
        go.GetComponent<Image>().color=color; return r;
    }
    private Text Label(Transform parent,string value,int size,Vector2 min,Vector2 max,Color color,TextAnchor align)
    {
        var go=new GameObject("Text",typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);
        var r=go.GetComponent<RectTransform>();r.anchorMin=new Vector2(0,1);r.anchorMax=new Vector2(1,1);r.pivot=new Vector2(.5f,1);r.offsetMin=min;r.offsetMax=max;
        var t=go.GetComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.color=color;t.alignment=align;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;
    }
    private void Button(Transform parent,string value,Vector2 min,Vector2 max,Color color,UnityEngine.Events.UnityAction action)
    {
        var go=new GameObject(value,typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(parent,false);
        var r=go.GetComponent<RectTransform>();r.anchorMin=new Vector2(0,1);r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.offsetMin=min;r.offsetMax=max;
        go.GetComponent<Image>().color=color;go.GetComponent<Button>().onClick.AddListener(action);
        var txt=Label(go.transform,value,14,new Vector2(4,-2),new Vector2(-4,-2),Color.white,TextAnchor.MiddleCenter);txt.rectTransform.anchorMin=Vector2.zero;txt.rectTransform.anchorMax=Vector2.one;txt.rectTransform.offsetMin=Vector2.zero;txt.rectTransform.offsetMax=Vector2.zero;
    }
    private void Inspect(string id,string title,string body)
    {
        found.Add(id);status.text="FOUND: "+found.Count+" / 3";Show("▶ "+title+"\n"+body);
    }
    private void Solve()
    {
        if(found.Count<3) Show("ОТКАЗ: недостаточно данных. Изучите все три материала.");
        else Show("СВЕРКА ЗАВЕРШЕНА — КОНФЛИКТ НАЙДЕН\n\nЖурнал указывает на MOTH, фотография подтверждает присутствие второго человека, а записка предупреждает о подмене времени. Официальный отчёт сфальсифицирован изнутри.\n\nНОВЫЙ ОБЪЕКТИВ: найдите, кто выдал временный ключ.");
    }
    private void Show(string message) { if(detail!=null) detail.text=message; }
}
