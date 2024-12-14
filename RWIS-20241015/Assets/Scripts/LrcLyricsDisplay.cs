using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class LrcLyricsDisplay : MonoBehaviour
{
    public TextMeshProUGUI[] textLines; // �̎���\������TextMeshProUGUI�I�u�W�F�N�g�i3�s���j
    public string lrcFileName = "Lrc-BirthdaySong.txt"; // LRC�t�@�C�����iAssets�t�H���_���j
    private List<LyricLine> lyrics = new List<LyricLine>(); // �̎������i�[���郊�X�g
    private int currentLyricIndex = 0; // ���݂̉̎��C���f�b�N�X
    private float timeInit = 0; // �̂̎n�܂�̎���
    private Color[] colors = { Color.red, Color.green, Color.yellow }; // �g�p����3�F
    private string[] colorNames = { "Red", "Green", "Yellow" }; // �F��

    [System.Serializable]
    public class LyricPart
    {
        public string word; // �P��
        public Color color; // ���蓖�Ă�ꂽ�F
    }

    [System.Serializable]
    public class LyricLine
    {
        public float time; // �\�������i�b�P�ʁj
        public string text; // �̎����e
        public List<LyricPart> parts = new List<LyricPart>(); // �P�ꂲ�Ƃ̐F���
    }

    void Start()
    {
        LoadLrcFile(); // LRC�t�@�C����ǂݍ���
        AssignRandomColors(); // �P�ꂲ�ƂɃ����_���ɐF�����蓖��
        ExportColorLog(); // �F���������L�^
        lyrics.Add(new LyricLine { time = timeInit, text = "" });
        UpdateLyricsDisplay(); // �����\�����X�V
    }

    void Update()
    {
        // ���݂̎����Ɋ�Â��ĉ̎����X�V
        float currentTime = Time.timeSinceLevelLoad;

        // ���̉̎��s�ɐi�ނׂ��^�C�~���O���m�F
        if (currentLyricIndex < lyrics.Count - 1 && currentTime >= lyrics[currentLyricIndex + 1].time)
        {
            currentLyricIndex++;
            UpdateLyricsDisplay();
        }
    }

    void LoadLrcFile()
    {
        string path = Path.Combine(Application.dataPath, lrcFileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"LRC file not found: {path}");
            return;
        }

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            // LRC�`�����p�[�X���鐳�K�\��
            if (System.Text.RegularExpressions.Regex.IsMatch(line, @"\[\d+:\d+\.\d+\]"))
            {
                // �����������擾
                string timePart = line.Substring(1, line.IndexOf("]") - 1);
                string[] timeComponents = timePart.Split(':');
                float minutes = float.Parse(timeComponents[0]);
                float seconds = float.Parse(timeComponents[1]);
                float totalSeconds = timeInit + minutes * 60 + seconds;

                // �̎��������擾
                string textPart = line.Substring(line.IndexOf("]") + 1);

                // ���X�g�ɒǉ�
                lyrics.Add(new LyricLine { time = totalSeconds, text = textPart });
            }
        }

        Debug.Log($"Loaded {lyrics.Count} lyrics from {lrcFileName}");
    }

    void AssignRandomColors()
    {
        foreach (var line in lyrics)
        {
            string[] words = line.text.Split(' '); // �P�ꂲ�Ƃɕ���
            foreach (var word in words)
            {
                int randomIndex = Random.Range(0, colors.Length); // �����_���ŐF��I��
                line.parts.Add(new LyricPart { word = word, color = colors[randomIndex] });
            }
        }
    }

    void ExportColorLog()
    {
        string logPath = Path.Combine(Application.dataPath, "LyricsColorLog.txt");
        using (StreamWriter writer = new StreamWriter(logPath))
        {
            writer.WriteLine("Lyrics Color Log:");
            foreach (var line in lyrics)
            {
                writer.WriteLine($"[{line.time:00.00}]");
                foreach (var part in line.parts)
                {
                    string colorName = ColorToName(part.color);
                    writer.WriteLine($"  \"{part.word}\" - {colorName}");
                }
            }
        }
        Debug.Log($"Color log saved to {logPath}");
    }

    string ColorToName(Color color)
    {
        if (color == Color.red) return "RED";
        if (color == Color.green) return "GREEN";
        if (color == Color.yellow) return "Yellow";
        return "UNKNOWN";
    }

    void UpdateLyricsDisplay()
    {
        // �^�񒆂̍s���X�V���邽�߂̃C���f�b�N�X
        int middleLineIndex = 1;

        for (int i = 0; i < textLines.Length; i++)
        {
            // �\������̎��s������i�O��1�s + ���ݍs�j
            int lyricIndex = currentLyricIndex + i - middleLineIndex;

            if (lyricIndex >= 0 && lyricIndex < lyrics.Count)
            {
                // �e�L�X�g��F�t���ō\�z
                string coloredText = "";
                foreach (var part in lyrics[lyricIndex].parts)
                {
                    string hexColor = ColorUtility.ToHtmlStringRGB(part.color);
                    coloredText += $"<color=#{hexColor}>{part.word}</color> ";
                }

                textLines[i].text = coloredText.Trim();

                // �^�񒆂̍s�͕s�����A����ȊO�͔�����
                if (i == middleLineIndex)
                {
                    textLines[i].color = new Color(1f, 1f, 1f, 1f); // �s����
                }
                else
                {
                    textLines[i].color = new Color(1f, 1f, 1f, 0.2f); // ������
                }
            }
            else
            {
                // �̎����Ȃ��ꍇ�͋󔒂ɐݒ�
                textLines[i].text = "";
            }
        }
    }
}
