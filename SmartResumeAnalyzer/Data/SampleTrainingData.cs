using System.Collections.Generic;

namespace SmartResumeAnalyzer.Data;

public static class SampleTrainingData
{
    /// <summary>
    /// Sample training data for ML model initialization
    /// </summary>
    public static List<(string Text, string[] Skills)> GetSampleTrainingData()
    {
        return new List<(string, string[])>
        {
            (
                "Proficient in C# and .NET frameworks with expertise in ASP.NET Core",
                new[] { "C#", ".NET", "ASP.NET" }
            ),
            (
                "Full-stack developer with JavaScript, React, and Node.js experience",
                new[] { "JavaScript", "React", "Node.js" }
            ),
            (
                "Python developer specializing in machine learning with TensorFlow and scikit-learn",
                new[] { "Python", "Machine Learning", "TensorFlow", "scikit-learn" }
            ),
            (
                "Database expert with SQL Server, PostgreSQL, and MongoDB",
                new[] { "SQL Server", "PostgreSQL", "MongoDB", "SQL" }
            ),
            (
                "Cloud architect with AWS and Azure certification",
                new[] { "AWS", "Azure", "Cloud" }
            ),
            (
                "DevOps engineer experienced with Docker, Kubernetes, and CI/CD pipelines",
                new[] { "Docker", "Kubernetes", "CI/CD" }
            ),
            (
                "Java developer with Spring Boot and microservices architecture experience",
                new[] { "Java", "Spring Boot", "Microservices" }
            ),
            (
                "Data analyst skilled in SQL, Tableau, and Python for data visualization",
                new[] { "SQL", "Tableau", "Python", "Data Analysis" }
            ),
            (
                "Mobile developer with Swift and Kotlin for iOS and Android development",
                new[] { "Swift", "Kotlin", "Mobile Development" }
            ),
            (
                "QA automation engineer with Selenium and TestNG expertise",
                new[] { "Selenium", "TestNG", "QA Automation" }
            )
        };
    }

    /// <summary>
    /// Sample job descriptions for matching
    /// </summary>
    public static Dictionary<string, string> GetSampleJobDescriptions()
    {
        return new Dictionary<string, string>
        {
            {
                "Senior .NET Developer", @"
                We are looking for an experienced .NET developer with:
                - 5+ years of C# programming experience
                - ASP.NET Core expertise
                - SQL Server knowledge
                - Git version control
                - Experience with Azure and cloud deployments
                - Strong problem-solving skills
                "
            },
            {
                "Frontend Developer", @"
                Join our team as a Frontend Developer:
                - React or Angular expertise
                - JavaScript/TypeScript proficiency
                - CSS and responsive design
                - REST API integration
                - Git and agile development
                - Experience with webpack or build tools
                "
            },
            {
                "Full-Stack Developer", @"
                We need a talented Full-Stack Developer:
                - Node.js backend development
                - React or Vue.js frontend
                - MongoDB or SQL database experience
                - RESTful API design
                - Docker containerization
                - AWS or Azure cloud experience
                "
            },
            {
                "Data Scientist", @"
                Looking for a skilled Data Scientist:
                - Python programming expertise
                - Machine Learning libraries (TensorFlow, scikit-learn)
                - Statistical analysis and modeling
                - Data visualization (Matplotlib, Tableau)
                - SQL for data extraction
                - Large dataset handling
                "
            },
            {
                "DevOps Engineer", @"
                We are hiring a DevOps Engineer:
                - Docker and container orchestration
                - Kubernetes experience
                - CI/CD pipeline setup (Jenkins, GitLab CI)
                - Infrastructure as code (Terraform, Ansible)
                - Cloud platforms (AWS, Azure, GCP)
                - Monitoring and logging tools
                "
            }
        };
    }

    /// <summary>
    /// Sample resumes for testing
    /// </summary>
    public static Dictionary<string, string> GetSampleResumes()
    {
        return new Dictionary<string, string>
        {
            {
                "resume1.txt", @"
                John Smith
                john.smith@email.com | (555) 123-4567

                PROFESSIONAL SUMMARY
                Experienced Full-Stack Developer with 7 years of expertise in C# and .NET technologies.
                Proven track record of delivering scalable applications and leading development teams.

                EDUCATION
                Bachelor of Science in Computer Science
                State University, Graduated 2017

                EXPERIENCE
                Senior Developer - TechCorp (2022-Present)
                - Developed ASP.NET Core microservices using C# and Entity Framework
                - Implemented Azure DevOps CI/CD pipeline
                - Mentored junior developers
                - Improved application performance by 40%

                Developer - SoftwareWorks (2019-2022)
                - Built RESTful APIs using .NET
                - Implemented SQL Server database solutions
                - Collaborated with frontend team using Git

                Junior Developer - StartupXYZ (2017-2019)
                - Developed web applications using C# and ASP.NET
                - Worked with SQL Server databases
                - Participated in code reviews

                SKILLS
                Languages: C#, JavaScript, Python
                Frameworks: .NET, ASP.NET Core, Entity Framework
                Databases: SQL Server, MongoDB, PostgreSQL
                Cloud: Azure, AWS
                Tools: Git, Visual Studio, Docker, Kubernetes
                Soft Skills: Leadership, Communication, Problem Solving

                CERTIFICATIONS
                Microsoft Certified: Azure Developer Associate (2023)
                Microsoft Certified: .NET Developer (2022)
                "
            },
            {
                "resume2.txt", @"
                Sarah Johnson
                sarah.j@email.com | (555) 987-6543

                SUMMARY
                Passionate React developer with 5 years of frontend development experience.
                Specialized in building responsive, user-friendly web applications.

                EDUCATION
                Master's Degree in Information Technology
                Tech University, 2019

                PROFESSIONAL EXPERIENCE
                Senior Frontend Engineer - WebSolutions (2021-Present)
                - Developed React-based applications with Redux state management
                - Implemented responsive designs using CSS and Tailwind CSS
                - Integrated REST APIs and GraphQL endpoints
                - Mentored 3 junior developers

                Frontend Developer - DesignStudio (2018-2021)
                - Built interactive web applications using React and Vue.js
                - Collaborated with UX/UI designers
                - Worked with Webpack and npm build tools
                - Implemented unit tests with Jest

                Developer - WebAgency (2017-2018)
                - Developed HTML, CSS, and JavaScript websites
                - Converted designs to responsive web pages

                KEY SKILLS
                Frontend: React, Vue.js, Angular, JavaScript, TypeScript
                Styling: CSS3, SASS, Bootstrap, Tailwind CSS
                Build Tools: Webpack, npm, gulp
                Testing: Jest, React Testing Library
                Version Control: Git, GitHub
                Other: Responsive Design, Accessibility, REST APIs

                CERTIFICATIONS
                React Developer Certificate (2022)
                JavaScript Advanced Developer (2021)
                "
            },
            {
                "resume3.txt", @"
                Michael Chen
                michael.chen@email.com | (555) 456-7890

                OBJECTIVE
                Motivated Data Scientist with strong background in machine learning and statistical analysis.
                Seeking to leverage Python and ML expertise to develop predictive models.

                EDUCATION
                Bachelor of Science in Mathematics
                University of Science, 2018

                WORK HISTORY
                Data Scientist - DataCorp Analytics (2020-Present)
                - Developed machine learning models using TensorFlow and scikit-learn
                - Performed data analysis on large datasets using Python and Pandas
                - Created data visualizations with Matplotlib and Tableau
                - Achieved 92% accuracy in classification models

                Junior Data Analyst - AnalyticsPlus (2018-2020)
                - Extracted and analyzed data using SQL
                - Built dashboards using Tableau
                - Documented data pipelines and analysis results

                TECHNICAL SKILLS
                Programming: Python, R, SQL
                ML Libraries: TensorFlow, scikit-learn, Keras, PyTorch
                Data Tools: Pandas, NumPy, Matplotlib, Seaborn
                Databases: PostgreSQL, MySQL, MongoDB
                Visualization: Tableau, Power BI, Matplotlib
                Big Data: Spark, Hadoop basics
                Cloud: AWS, GCP

                CERTIFICATIONS
                Google Cloud Data Engineer (2023)
                AWS Machine Learning Specialty (2022)
                "
            }
        };
    }
}
