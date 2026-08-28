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
            ? new[] { "Kortskogen", "Koble kompisene", "Puslemesteren" }
            : course.Grade <= 7
                ? new[] { "Kortjakten", "Koblingskartet", "Fagpuslespillet" }
                : new[] { "Fagduellen", "Sammenhengslabben", "Analysepuslespillet" };
        var visualCues = new[] { "◆", "●", "▲", "■", "✦", "⬟", "◈", "⬢", "✚", "◇" };

        var sortTargets = vocabulary.Targets.Take(3).ToArray();
        var sortOffset = (course.Grade * 3 + course.SortOrder * 5 + 1) % easyDistractors.Length;
        var sortDistractors = easyDistractors.Concat(easyDistractors).Skip(sortOffset).Take(3).ToArray();
        var sortCards = sortTargets.Select((cardText, index) => new GameCard
            {
                Text = cardText, IsTarget = true, SortOrder = index * 2 + 1, VisualCue = visualCues[index]
            })
            .Concat(sortDistractors.Select((cardText, index) => new GameCard
            {
                Text = cardText, SortOrder = index * 2 + 2, VisualCue = visualCues[index + 3]
            })).OrderBy(x => x.SortOrder).ToList();

        var matchingCards = vocabulary.Pairs.SelectMany((pair, index) => new[]
        {
            new GameCard { Text = pair.Prompt, IsTarget = true, PairKey = $"pair-{index}", SortOrder = index + 1, VisualCue = "◆" },
            new GameCard { Text = pair.Answer, PairKey = $"pair-{index}", SortOrder = index + 1 + vocabulary.Pairs.Count, VisualCue = "●" }
        }).ToList();

        var jigsawCards = vocabulary.SequencePieces.Select((piece, index) => new GameCard
        {
            Text = piece, IsTarget = true, CorrectPosition = index + 1, SortOrder = index + 1,
            VisualCue = visualCues[index]
        }).ToList();

        return new CourseGame
        {
            Course = course,
            Title = $"Fagoppdrag: {course.Title}",
            Intro = vocabulary.Intro,
            Levels =
            [
                new GameLevel
                {
                    LevelNumber = 1, Mode = GameLevelMode.CardSort, Title = titles[0], MaxPoints = points[0],
                    Instructions = $"Finn de tre kortene som hører direkte til «{course.Title}». Symbolene er bare dekorasjon – bruk fagkunnskapen.",
                    Cards = sortCards
                },
                new GameLevel
                {
                    LevelNumber = 2, Mode = GameLevelMode.Matching, Title = titles[1], MaxPoints = points[1],
                    Instructions = "Koble hver etikett til riktig forklaring. Velg først et kort til venstre og deretter svaret til høyre; linjene viser koblingene dine.",
                    Cards = matchingCards
                },
                new GameLevel
                {
                    LevelNumber = 3, Mode = GameLevelMode.Jigsaw, Title = titles[2], MaxPoints = points[2],
                    Instructions = "Bygg det faglige puslespillet: trykk brikkene i den rekkefølgen en grundig problemløser bør arbeide.",
                    Cards = jigsawCards
                }
            ]
        };
    }
}
