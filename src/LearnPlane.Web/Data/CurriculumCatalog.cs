using LearnPlane.Web.Models;

namespace LearnPlane.Web.Data;

public static class CurriculumCatalog
{
    private sealed record Topic(string Name, string Fact);

    private static readonly IReadOnlyDictionary<string, Topic[]> Topics = new Dictionary<string, Topic[]>
    {
        ["Norsk"] =
        [
            new("Leseforståelse", "En aktiv leser stopper opp, stiller spørsmål og oppsummerer teksten med egne ord."),
            new("Fortelling og sjanger", "En fortelling har ofte en begynnelse, en utfordring og en avslutning."),
            new("Rettskriving og grammatikk", "Grammatikk beskriver hvordan ord og setninger er bygd opp og virker sammen."),
            new("Kildekritikk og argumentasjon", "En troverdig påstand støttes av relevante kilder og tydelige begrunnelser."),
            new("Språklig mangfold", "Norge har bokmål, nynorsk, samiske språk og mange dialekter og minoritetsspråk.")
        ],
        ["Matematikk"] =
        [
            new("Tall og regning", "Tall kan deles opp og settes sammen på flere måter når vi regner."),
            new("Måling og geometri", "Geometri handler om former, plassering, vinkler, lengde, areal og volum."),
            new("Brøk, desimaltall og prosent", "Brøk, desimaltall og prosent er ulike måter å beskrive deler av en helhet på."),
            new("Algebra og likninger", "En likning viser at to uttrykk har samme verdi, og en ukjent kan finnes ved balanse."),
            new("Statistikk og sannsynlighet", "Statistikk hjelper oss å samle, vise og tolke data, mens sannsynlighet beskriver hvor mulig noe er.")
        ],
        ["Engelsk"] =
        [
            new("Everyday English", "We learn language by listening, speaking, reading and trying new words in context."),
            new("Reading and stories", "A reader can use the title, context and key words to understand a text."),
            new("Grammar and writing", "Clear English sentences normally need a subject and a verb."),
            new("English-speaking cultures", "English is used in many communities, with different cultures and varieties."),
            new("Discussion and presentation", "A good presentation has a clear message, examples and language suited to the audience.")
        ],
        ["Naturfag"] =
        [
            new("Kropp og helse", "Kroppen trenger variert mat, aktivitet, hvile og søvn for å fungere godt."),
            new("Dyr, planter og økosystemer", "I et økosystem påvirker levende organismer og miljøet hverandre."),
            new("Stoff, energi og krefter", "Energi kan overføres og omformes, men den blir ikke borte."),
            new("Jorda, klima og bærekraft", "Klima beskriver værmønstre over lang tid og påvirkes av både natur og mennesker."),
            new("Vitenskapelig metode", "En undersøkelse blir etterprøvbar når spørsmål, metode, observasjoner og konklusjon dokumenteres.")
        ],
        ["Samfunnsfag"] =
        [
            new("Familie og nærmiljø", "Et lokalsamfunn består av mennesker, steder, tjenester og regler som virker sammen."),
            new("Kart, landskap og ressurser", "Kart bruker symboler, målestokk og retninger for å vise steder og sammenhenger."),
            new("Historie og kildebruk", "Historiske kilder må undersøkes for opphav, formål og hva de faktisk kan fortelle."),
            new("Demokrati og medborgerskap", "I et demokrati kan innbyggerne påvirke, og flertallet må respektere mindretallets rettigheter."),
            new("Økonomi og globalisering", "Valg om produksjon og forbruk påvirker mennesker, økonomi og miljø lokalt og globalt.")
        ],
        ["KRLE"] =
        [
            new("Høytider og tradisjoner", "Høytider uttrykker tro, livssyn, historie og fellesskap på ulike måter."),
            new("Etikk og vennskap", "Etiske spørsmål handler om hvordan valg påvirker oss selv, andre og samfunnet."),
            new("Religioner og livssyn", "Religioner og livssyn gir ulike svar på spørsmål om mening, virkelighet og hvordan vi bør leve."),
            new("Filosofi og store spørsmål", "Filosofi undersøker grunnleggende spørsmål ved å definere begreper og vurdere argumenter."),
            new("Menneskerettigheter", "Menneskerettighetene gjelder alle mennesker og beskytter blant annet frihet, likeverd og deltakelse.")
        ],
        ["Kunst og håndverk"] =
        [
            new("Farge, form og tegning", "Farger, former, linjer og kontraster kan brukes bevisst for å skape uttrykk."),
            new("Materialer og teknikker", "Godt håndverk krever at verktøy og materialer brukes trygt og tilpasset oppgaven."),
            new("Designprosess", "En designprosess går fra behov og idé via utprøving til vurdering og forbedring."),
            new("Kunst, arkitektur og kultur", "Kunst og arkitektur kan fortelle om tid, sted, identitet og samfunn."),
            new("Bærekraftig design", "Bærekraftig design vurderer levetid, reparasjon, ressursbruk og gjenbruk.")
        ],
        ["Musikk"] =
        [
            new("Rytme og puls", "Puls er det jevne slaget i musikken, mens rytme er mønsteret av korte og lange lyder."),
            new("Sang og samspill", "Godt samspill krever lytting, felles puls og tilpasning til de andre."),
            new("Melodi og harmoni", "Melodi er toner etter hverandre, mens harmoni oppstår når toner klinger sammen."),
            new("Komposisjon", "Å komponere er å skape og organisere musikalske ideer til en helhet."),
            new("Musikk, kultur og teknologi", "Musikk formes av kultur, historie, teknologi og hvordan den brukes.")
        ],
        ["Kroppsøving"] =
        [
            new("Lek og bevegelse", "Lek utvikler bevegelsesglede, samarbeid og evnen til å prøve løsninger."),
            new("Fair play og samarbeid", "Fair play betyr å følge regler, inkludere andre og vise respekt i medgang og motgang."),
            new("Friluftsliv og svømming", "Trygg aktivitet ute og i vann krever planlegging, risikovurdering og riktige ferdigheter."),
            new("Trening og helse", "Trening påvirker utholdenhet, styrke, bevegelighet og psykisk velvære."),
            new("Livredning og førstehjelp", "Ved en ulykke må man sikre stedet, varsle hjelp og gi forsvarlig førstehjelp.")
        ],
        ["Mat og helse"] =
        [
            new("Matglede og kjøkkenhygiene", "Rene hender, adskilte råvarer og riktig temperatur reduserer risiko for sykdom."),
            new("Kosthold og næringsstoffer", "Et variert kosthold gir kroppen energi og ulike næringsstoffer den trenger."),
            new("Oppskrifter og måling", "En oppskrift beskriver råvarer, mengder, rekkefølge, tid og temperatur."),
            new("Matkultur", "Matvaner og måltider formes av geografi, historie, tro, identitet og tilgang på råvarer."),
            new("Bærekraftige matvalg", "Sesongvarer, mindre matsvinn og bevisste råvarevalg kan redusere miljøbelastningen.")
        ],
        ["Fremmedspråk"] =
        [
            new("Hilsener og presentasjon", "Hyppig bruk av enkle fraser gjør det lettere å delta i samtaler på et nytt språk."),
            new("Ordforråd og uttale", "Ord læres best når de brukes gjentatte ganger i meningsfulle sammenhenger."),
            new("Grunnleggende grammatikk", "Mønstre for ordstilling og bøying hjelper oss å lage forståelige setninger."),
            new("Kultur og samfunn", "Språklæring gir tilgang til ulike perspektiver, levemåter og kulturuttrykk."),
            new("Samtale og tekst", "Strategier som omskriving og spørsmål holder kommunikasjonen i gang når et ord mangler.")
        ],
        ["Utdanningsvalg"] =
        [
            new("Interesser og styrker", "Å kjenne egne interesser, verdier og styrker gjør utdanningsvalg mer bevisste."),
            new("Videregående opplæring", "Videregående opplæring har studieforberedende og yrkesfaglige utdanningsprogram."),
            new("Arbeidsliv og yrker", "Yrker krever ulike kombinasjoner av kompetanse, samarbeid, ansvar og videre læring."),
            new("Valg og konsekvenser", "Gode valg bygger på pålitelig informasjon, alternativer og vurdering av konsekvenser.")
        ],
        ["Valgfag"] =
        [
            new("Idé og prosjekt", "Et prosjekt trenger et tydelig mål, rollefordeling, framdriftsplan og evaluering."),
            new("Praktisk skapende arbeid", "Praktisk arbeid forbedres gjennom utprøving, tilbakemelding og nye versjoner."),
            new("Samarbeid og formidling", "God formidling tilpasses målgruppe, formål og valgt medium."),
            new("Entreprenørskap", "Entreprenørskap handler om å se behov og utvikle en løsning som skaper verdi.")
        ],
        ["Arbeidslivsfag"] =
        [
            new("Arbeidsoppdrag og kvalitet", "Et arbeidsoppdrag bør ha krav, plan, trygg utførelse og kontroll av resultatet."),
            new("Helse, miljø og sikkerhet", "HMS forebygger skade gjennom risikovurdering, riktig utstyr og sikre rutiner."),
            new("Samarbeid på arbeidsplassen", "Tydelig kommunikasjon, ansvar og respekt er grunnlag for godt samarbeid."),
            new("Råvarer, verktøy og økonomi", "Valg av materiale og metode påvirker kvalitet, kostnad, tidsbruk og miljø.")
        ]
    };

    public static IReadOnlyList<string> SubjectsForGrade(int grade)
    {
        var common = new List<string>
        {
            "Norsk", "Matematikk", "Engelsk", "Naturfag", "Samfunnsfag", "KRLE",
            "Kunst og håndverk", "Musikk", "Kroppsøving", "Mat og helse"
        };
        if (grade >= 8)
            common.AddRange(["Fremmedspråk", "Utdanningsvalg", "Valgfag", "Arbeidslivsfag"]);
        return common;
    }

    public static IEnumerable<Course> CreateCourses()
    {
        for (var grade = 1; grade <= 10; grade++)
        {
            foreach (var subject in SubjectsForGrade(grade))
            {
                var topics = Topics[subject];
                var baseTopicIndex = grade >= 8 && subject is "Fremmedspråk" or "Utdanningsvalg" or "Valgfag" or "Arbeidslivsfag"
                    ? grade - 8
                    : grade <= 2 ? 0 : grade <= 4 ? 1 : grade <= 7 ? 2 : 3;
                for (var courseNumber = 0; courseNumber < 2; courseNumber++)
                {
                    var topic = topics[(baseTopicIndex + courseNumber) % topics.Length];
                    var alternatives = topics.Where(x => x != topic).Take(3).Select(x => x.Name).ToArray();
                    var difficulty = grade <= 3 ? CourseDifficulty.Lett
                        : grade <= 7 ? CourseDifficulty.Middels : CourseDifficulty.Utfordrende;
                    yield return BuildCourse(grade, subject, topic, alternatives, difficulty, courseNumber);
                }
            }
        }
    }

    private static Course BuildCourse(int grade, string subject, Topic topic, string[] alternatives,
        CourseDifficulty difficulty, int sortOrder)
    {
        var course = new Course
        {
            Grade = grade,
            Subject = subject,
            Title = topic.Name,
            Summary = $"Et kort kurs i {topic.Name.ToLowerInvariant()} for {grade}. trinn, med forklaring, aktivitet og quiz.",
            Content = $"""
                <h2>Dette skal du lære</h2>
                <p>I dette kurset arbeider du med <strong>{topic.Name.ToLowerInvariant()}</strong> i {subject.ToLowerInvariant()}.</p>
                <div class="fact-box"><strong>Husk:</strong> {topic.Fact}</div>
                <h2>Utforsk</h2>
                <p>Finn et eksempel fra hverdagen, en tekst eller en aktivitet som passer til temaet. Forklar med egne ord hva du legger merke til, og sammenlign med huskeregelen.</p>
                <h2>Prøv selv</h2>
                <ol><li>Skriv eller fortell hva du allerede vet.</li><li>Lag ett konkret eksempel.</li><li>Forklar eksemplet til en annen person.</li><li>Ta quizen og bruk forklaringene til å lære mer.</li></ol>
                <p class="source-note">Kurset er tematisk tilpasset LK20. Det er et læringssupplement og erstatter ikke skolens undervisningsplan.</p>
                """,
            Difficulty = difficulty,
            SortOrder = sortOrder,
            IsPublished = true
        };

        course.Questions =
        [
            Question(1, "Hva er hovedtemaet i dette kurset?", topic.Name, alternatives[0], alternatives[1], alternatives[2],
                $"Kurset handler om {topic.Name.ToLowerInvariant()}."),
            Question(2, "Hvilket utsagn oppsummerer huskeregelen best?", topic.Fact,
                "Det finnes bare én riktig måte å lære på.", "Detaljer er alltid viktigere enn sammenhenger.", "Man lærer uten å øve eller undersøke.", topic.Fact),
            Question(3, "Hva er en god måte å undersøke temaet videre på?",
                "Finne et eksempel og forklare det med egne ord", "Hoppe over eksempler", "Bare gjette uten å undersøke", "Unngå å snakke om det",
                "Konkrete eksempler og egne forklaringer gjør forståelsen synlig."),
            Question(4, "Hva bør du gjøre hvis du svarer feil i quizen?",
                "Lese forklaringen, prøve på nytt og finne et nytt eksempel", "Gi opp kurset", "Velge samme svar uten å tenke", "Skjule resultatet",
                "Feil er nyttig informasjon når du undersøker forklaringen og prøver igjen.")
        ];
        return course;
    }

    private static QuizQuestion Question(int sortOrder, string text, string correct, string wrong1, string wrong2,
        string wrong3, string explanation) => new()
        {
            SortOrder = sortOrder,
            Text = text,
            Explanation = explanation,
            Options =
        [
            new AnswerOption { Text = correct, IsCorrect = true, SortOrder = 1 },
            new AnswerOption { Text = wrong1, SortOrder = 2 },
            new AnswerOption { Text = wrong2, SortOrder = 3 },
            new AnswerOption { Text = wrong3, SortOrder = 4 }
        ]
        };
}
