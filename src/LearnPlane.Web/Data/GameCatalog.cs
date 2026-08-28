using LearnPlane.Web.Models;

namespace LearnPlane.Web.Data;

public static class GameCatalog
{
    private static readonly IReadOnlyDictionary<string, string[]> EasyDistractors = new Dictionary<string, string[]>
    {
        ["Norsk"] = ["hovedidé", "avsnitt", "forteller", "verb", "substantiv", "argument", "kilde", "dialekt", "rim", "sammenheng"],
        ["Matematikk"] = ["tallinje", "mønster", "vinkel", "areal", "brøk", "prosent", "variabel", "likhet", "diagram", "sannsynlighet"],
        ["Engelsk"] = ["sentence", "verb", "context", "character", "meaning", "audience", "culture", "pronunciation", "dialogue", "headline"],
        ["Naturfag"] = ["observasjon", "hypotese", "energi", "kraft", "celle", "næringskjede", "klima", "stoff", "måling", "økosystem"],
        ["Samfunnsfag"] = ["kart", "målestokk", "kilde", "demokrati", "ressurs", "tidslinje", "rettighet", "økonomi", "landskap", "medborgerskap"],
        ["KRLE"] = ["etikk", "tradisjon", "livssyn", "ritual", "likeverd", "filosofi", "respekt", "høytid", "rettighet", "fellesskap"],
        ["Kunst og håndverk"] = ["kontrast", "skisse", "form", "materiale", "tekstur", "design", "komposisjon", "gjenbruk", "verktøy", "funksjon"],
        ["Musikk"] = ["puls", "rytme", "melodi", "harmoni", "tempo", "dynamikk", "klang", "komposisjon", "samspill", "refreng"],
        ["Kroppsøving"] = ["balanse", "koordinasjon", "utholdenhet", "fair play", "samarbeid", "oppvarming", "livredning", "styrke", "bevegelse", "sikkerhet"],
        ["Mat og helse"] = ["hygiene", "råvare", "oppskrift", "næringsstoff", "temperatur", "måleenhet", "matsvinn", "sesong", "kosthold", "smak"],
        ["Fremmedspråk"] = ["hilsen", "uttale", "ordstilling", "samtale", "spørsmål", "kultur", "bøying", "ordforråd", "lytting", "presentasjon"],
        ["Utdanningsvalg"] = ["interesse", "styrke", "kompetanse", "yrke", "utdanning", "konsekvens", "arbeidsliv", "verdi", "alternativ", "mål"],
        ["Valgfag"] = ["prosjekt", "idé", "målgruppe", "prototype", "samarbeid", "framdrift", "formidling", "evaluering", "rolle", "løsning"],
        ["Arbeidslivsfag"] = ["HMS", "kvalitet", "verktøy", "risiko", "arbeidsoppdrag", "kostnad", "rutine", "ansvar", "materiale", "plan"]
    };

    public static CourseGame CreateGame(Course course)
    {
        var vocabulary = CurriculumCatalog.GetGameVocabulary(course);
        var easyDistractors = EasyDistractors
            .Where(x => x.Key != course.Subject)
            .SelectMany(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(x => !vocabulary.Targets.Contains(x, StringComparer.OrdinalIgnoreCase))
            .ToArray();
        var points = course.Difficulty switch
        {
            CourseDifficulty.Lett => new[] { 4, 6, 8 },
            CourseDifficulty.Middels => new[] { 6, 10, 14 },
            _ => new[] { 8, 14, 20 }
        };
        var titles = course.Grade <= 3
            ? new[] { "Ordskogen", "Skattejakten", "Mesterstien" }
            : course.Grade <= 7
                ? new[] { "Begrepsjakten", "Kodeknekkeren", "Mesteroppdraget" }
                : new[] { "Fagduellen", "Sammenhengslabben", "Ekspertnivået" };

        return new CourseGame
        {
            Course = course,
            Title = $"Fagoppdrag: {course.Title}",
            Intro = vocabulary.Intro,
            Levels = Enumerable.Range(1, 3).Select(levelNumber =>
            {
                var count = levelNumber switch { 1 => 3, 2 => 5, _ => 7 };
                var targets = vocabulary.Targets.Take(count).ToArray();
                var distractorPool = levelNumber == 1 ? easyDistractors : vocabulary.Distractors;
                var offset = (course.Grade * 3 + course.SortOrder * 5 + levelNumber) % Math.Max(1, distractorPool.Count);
                var distractors = distractorPool.Concat(distractorPool).Skip(offset).Take(count).ToArray();
                var cards = targets.Select((text, index) => new GameCard { Text = text, IsTarget = true, SortOrder = index * 2 + 1 })
                    .Concat(distractors.Select((text, index) => new GameCard { Text = text, IsTarget = false, SortOrder = index * 2 + 2 }))
                    .OrderBy(x => x.SortOrder).ToList();
                return new GameLevel
                {
                    LevelNumber = levelNumber,
                    Title = titles[levelNumber - 1],
                    Instructions = levelNumber == 1
                        ? $"Velg nøyaktig {count} grunnbegreper som hører til «{course.Title}» – ikke bare til andre skolefag."
                        : $"Velg nøyaktig {count} begreper som hører direkte til «{course.Title}». Fellene er nå ekte begreper fra andre temaer i {course.Subject.ToLowerInvariant()}.",
                    MaxPoints = points[levelNumber - 1],
                    Cards = cards
                };
            }).ToList()
        };
    }
}
