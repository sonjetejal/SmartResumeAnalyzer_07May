using System;

namespace SmartResumeAnalyzer.Models;

public class MLInputData
{
    public string Text { get; set; } = string.Empty;
}

public class MLOutputData
{
    public float[] Predictions { get; set; } = Array.Empty<float>();
    public float Score { get; set; }
}

public class SkillPrediction
{
    public bool IsSkill { get; set; }
    public float Confidence { get; set; }
}

public class CertificationPrediction
{
    public bool IsCertification { get; set; }
    public float Confidence { get; set; }
}
