using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartResumeAnalyzer.Data;

public static class SkillDatabase
{
    public static readonly Dictionary<string, string[]> SkillCategories = new()
    {
        {
            "Languages", new[]
            {
                "C#", "Python", "Java", "JavaScript", "TypeScript", "Go", "Rust",
                "C++", "PHP", "Ruby", "Kotlin", "Swift", "Objective-C", "Scala"
            }
        },
        {
            "Frontend", new[]
            {
                "React", "Angular", "Vue.js", "Svelte", "Next.js", "Nuxt.js",
                "HTML", "CSS", "Bootstrap", "Tailwind", "Material Design", "Webpack"
            }
        },
        {
            "Backend", new[]
            {
                ".NET", "ASP.NET", "Node.js", "Express", "Django", "Flask",
                "Spring", "Laravel", "Ruby on Rails", "FastAPI", "Fastify"
            }
        },
        {
            "Databases", new[]
            {
                "SQL Server", "MySQL", "PostgreSQL", "MongoDB", "Redis",
                "DynamoDB", "Cassandra", "Elasticsearch", "Firebase", "Oracle"
            }
        },
        {
            "Cloud Platforms", new[]
            {
                "AWS", "Azure", "Google Cloud", "Heroku", "DigitalOcean",
                "IBM Cloud", "Alibaba Cloud", "CloudFlare"
            }
        },
        {
            "DevOps", new[]
            {
                "Docker", "Kubernetes", "Jenkins", "GitLab CI/CD", "GitHub Actions",
                "Terraform", "Ansible", "CloudFormation", "Helm", "Docker Compose"
            }
        },
        {
            "Data Science & ML", new[]
            {
                "Machine Learning", "TensorFlow", "PyTorch", "Scikit-learn",
                "Pandas", "NumPy", "Matplotlib", "Keras", "OpenCV", "NLTK",
                "Spark", "Hadoop", "Data Analysis"
            }
        },
        {
            "Soft Skills", new[]
            {
                "Leadership", "Communication", "Problem Solving", "Team Work",
                "Project Management", "Time Management", "Adaptability", "Creativity",
                "Critical Thinking", "Attention to Detail", "Mentoring", "Negotiation"
            }
        },
        {
            "Tools & Platforms", new[]
            {
                "Git", "GitHub", "GitLab", "Jira", "Azure DevOps", "Confluence",
                "Slack", "Visual Studio", "VS Code", "IntelliJ", "Postman"
            }
        },
        {
            "Database Technologies", new[]
            {
                "Entity Framework", "Dapper", "LINQ", "ADO.NET", "Hibernate",
                "SQLAlchemy", "TypeORM", "Sequelize", "Knex.js"
            }
        }
    };

    public static readonly string[] Certifications = new[]
    {
        "AWS Solutions Architect", "AWS Developer Associate", "AWS SysOps Administrator",
        "Azure Solutions Architect", "Azure Administrator", "Azure Developer",
        "Google Cloud Professional", "Kubernetes Certified Administrator",
        "Project Management Professional (PMP)", "Certified Scrum Master (CSM)",
        "Certified ScrumProduct Owner (CSPO)", "Six Sigma",
        "CompTIA Security+", "Certified Ethical Hacker (CEH)",
        "CISSP", "OSCP", "Oracle Certified Associate",
        "Microsoft Certified: Azure Developer Associate",
        "Microsoft Certified: Azure Solutions Architect Expert"
    };

    public static List<(string Skill, string Category)> GetAllSkills()
    {
        var allSkills = new List<(string, string)>();
        
        foreach (var category in SkillCategories)
        {
            foreach (var skill in category.Value)
            {
                allSkills.Add((skill, category.Key));
            }
        }

        return allSkills;
    }

    public static string? GetSkillCategory(string skill)
    {
        foreach (var category in SkillCategories)
        {
            if (category.Value.Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase)))
            {
                return category.Key;
            }
        }

        return null;
    }

    public static bool IsCertification(string text)
    {
        return Certifications.Any(c => 
            text.Contains(c, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsKnownSkill(string skill)
    {
        return SkillCategories.Values.Any(skills => 
            skills.Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase)));
    }
}
