using System.Net;

namespace LearnPlane.Web.Data;

internal sealed record AdaptedTopicText(string Core, string Explanation, string Example, string Reminder);

internal sealed record LessonPedagogy(
    AdaptedTopicText Topic,
    string Summary,
    string Goal,
    string ExplanationHeading,
    string ExampleHeading,
    string MethodHeading,
    string StudyAdvice,
    string Method,
    IReadOnlyList<string> Steps,
    string Challenge,
    string ResponsePrompt,
    int TermCount,
    string VisualHtml);

internal static class AgeAdaptedPedagogy
{
    private static readonly IReadOnlyDictionary<(string Subject, string Topic), AdaptedTopicText> YoungTopics =
        new Dictionary<(string, string), AdaptedTopicText>
        {
            [("Norsk", "Leseforståelse")] = new("Ord og bilder gir oss spor om hva en tekst betyr.",
                "Se på bildet og overskriften. Les én setning om gangen. Stopp og fortell hva du fant ut.",
                "Mina tar på støvler. Jakken hennes er våt. Da kan vi tenke at det regner.",
                "Vi gjetter ikke helt fritt. Vi peker på et ord eller et bilde som hjelper oss."),
            [("Norsk", "Fortelling og sjanger")] = new("En fortelling har noen det handler om, et sted og noe som skjer.",
                "Først møter vi ofte en person. Så skjer det noe. Til slutt får vi vite hvordan det går.",
                "En rev mister votten sin, leter i skogen og finner den hos haren. Det er en liten fortelling.",
                "En fortelling kan være morsom, trist eller spennende. Den trenger ikke ende likt hver gang."),
            [("Matematikk", "Tall og regning")] = new("Tall viser hvor mange vi har, og vi kan sette sammen eller ta bort.",
                "Bruk fingre, klosser eller prikker. Tell rolig. Flytt én ting om gangen og se hva som skjer med antallet.",
                "Du har 4 klosser og får 2 til: ●●●● + ●● = 6 klosser.",
                "Tell tingene én gang til. Det siste tallet du sier, forteller hvor mange det er."),
            [("Matematikk", "Måling og geometri")] = new("Vi kan se på former og finne ut hvor lange eller store ting er.",
                "En trekant har tre sider. En firkant har fire. Legg en linjal fra kanten og start ved null når du måler.",
                "En blyant kan være 12 centimeter lang. En trekant kan lages av tre ispinner.",
                "Lengde og form er ikke det samme. To trekanter kan ha ulik størrelse og fortsatt være trekanter."),
            [("Engelsk", "Everyday English")] = new("Vi bruker korte engelske ord og setninger for å hilse og svare.",
                "Lytt etter ord du kjenner. Se på ansikt og kropp. Svar med en hel liten frase.",
                "– Hello! My name is Sam. – Hi Sam! I’m Ada.",
                "Du trenger ikke forstå hvert ord. Du kan si: “Can you say it again, please?”"),
            [("Engelsk", "Reading and stories")] = new("Ord og bilder hjelper oss å forstå en engelsk fortelling.",
                "Finn navn, ting og handlinger du kjenner. Se hva som skjer først og etterpå.",
                "“The dog runs to Mia.” Bildet viser en hund som løper. Da kan vi koble ord og bilde.",
                "Et ukjent ord betyr ikke at hele teksten er umulig. Bruk resten som hjelp."),
            [("Naturfag", "Kropp og helse")] = new("Kroppen har mange deler som samarbeider.",
                "Hjertet slår, lungene hjelper oss å puste, og musklene gjør at vi kan bevege oss. Søvn og mat hjelper kroppen.",
                "Når du løper, slår hjertet fortere og du puster mer. Kroppen arbeider hardt.",
                "Kropper er forskjellige. Det finnes ikke én mat eller øvelse som passer likt for alle."),
            [("Naturfag", "Dyr, planter og økosystemer")] = new("Dyr og planter trenger et sted å leve, mat, vann og lys.",
                "En plante bruker lys. En bie finner mat i blomsten. Fuglen kan spise insektet. Naturen henger sammen.",
                "Sol → blomst → bie. Uten blomster får bien mindre mat.",
                "Et dyr bor ikke hvor som helst. Vi ser etter hva det trenger på levestedet sitt."),
            [("Samfunnsfag", "Familie og nærmiljø")] = new("Vi hører til i familier og steder som kan være ulike.",
                "I nærmiljøet kan vi finne hjem, skole, vei, butikk, park og mennesker som hjelper hverandre.",
                "Ved skolen vil barna ha lekeplass, mens naboen ønsker ro. Begge kan fortelle hva de trenger.",
                "Familier ser ikke alltid like ut. Ulike familier er like mye familier."),
            [("Samfunnsfag", "Kart, landskap og ressurser")] = new("Et kart er en liten tegning av et sted sett ovenfra.",
                "Farger og tegn viser vei, vann og hus. En tegnforklaring forteller hva symbolene betyr.",
                "En blå strek kan være en elv. En svart firkant kan være skolen.",
                "Kartet viser ikke alt som finnes. Det viser det vi trenger for å finne fram."),
            [("KRLE", "Høytider og tradisjoner")] = new("Mennesker markerer viktige dager på mange måter.",
                "En høytid kan ha fortellinger, mat, lys, musikk, bønn eller tid med familien. Ikke alle gjør det samme.",
                "Noen tenner lys i en høytid. Andre samles til et måltid. Vi spør og lytter med respekt.",
                "Vi sier «noen feirer slik», ikke at alle i en religion gjør akkurat det samme."),
            [("KRLE", "Etikk og vennskap")] = new("Etikk handler om å tenke over hva som er godt og rett å gjøre.",
                "Vi kan stoppe, se hvem som blir berørt, lytte og velge en handling som er trygg og rettferdig.",
                "Hvis noen står alene, kan du spørre om de vil være med. Da viser du omsorg.",
                "Å være en god venn betyr ikke å holde på en hemmelighet som gjør noen utrygge."),
            [("Kunst og håndverk", "Farge, form og tegning")] = new("Linjer, former og farger kan vise en idé eller en følelse.",
                "Prøv rette og bølgende linjer. Bland farger. Lag store og små former og se hva øyet legger merke til.",
                "En stor gul sirkel kan være en sol. Blå bølgelinjer kan vise vann.",
                "En tegning trenger ikke ligne et fotografi. Dine bevisste valg kan gjøre bildet godt."),
            [("Kunst og håndverk", "Materialer og teknikker")] = new("Papir, tre, tekstil og leire kjennes og virker forskjellig.",
                "Bøy, kjenn og prøv materialet forsiktig. Velg verktøy som passer, og bruk det på en trygg måte.",
                "Papir kan brettes. Leire kan formes. Stoff kan klippes og sys.",
                "Ett verktøy passer ikke til alt. Stopp og spør før du bruker et nytt verktøy."),
            [("Musikk", "Rytme og puls")] = new("Pulsen er jevne slag. Rytmen er mønsteret vi klapper eller spiller.",
                "Gå jevnt: én–to–én–to. Klapp så et mønster over stegene: klapp–klapp–pause.",
                "Føttene går jevnt. Hendene klapper kort–kort–lang. Da hører du puls og rytme.",
                "Puls og rytme er ikke det samme. Pulsen holder seg jevn mens rytmen kan skifte."),
            [("Musikk", "Sang og samspill")] = new("Når vi lager musikk sammen, lytter vi og holder samme puls.",
                "Start og stopp sammen. Syng eller spill passe sterkt, slik at alle delene kan høres.",
                "Hvis trommen dekker sangen, kan trommen spille svakere.",
                "Alle trenger ikke gjøre det samme. Ulike roller kan passe godt sammen."),
            [("Kroppsøving", "Lek og bevegelse")] = new("Vi lærer med kroppen når vi løper, hopper, balanserer og leker.",
                "Se hvor du skal. Prøv rolig først. Bøy knærne, bruk armene og finn balansen.",
                "Gå på en strek, hopp over en ring og kast en myk ball til en venn.",
                "Raskest er ikke alltid best. Trygg kontroll og glede er også mestring."),
            [("Kroppsøving", "Fair play og samarbeid")] = new("Fair play er å leke trygt, rettferdig og vennlig sammen.",
                "Følg reglene, vent på tur, hjelp laget og bruk ord som gjør andre trygge.",
                "Hvis ballen var ute, sier du fra – også når det andre laget får ballen.",
                "Fair play er mer enn å ikke jukse. Det handler også om å inkludere andre."),
            [("Mat og helse", "Matglede og kjøkkenhygiene")] = new("Vi lager mat med rene hender, trygge redskaper og nysgjerrige sanser.",
                "Vask hendene. Rydd plassen. Smak, lukt og kjenn. Be om hjelp med varme og skarpe redskaper.",
                "Vi vasker et eple, skjærer med hjelp og smaker på en liten bit.",
                "Mat kan ha bakterier selv om den lukter vanlig. Derfor følger vi hygienereglene."),
            [("Mat og helse", "Kosthold og næringsstoffer")] = new("Ulik mat gir kroppen ulike ting den trenger.",
                "Vi kan spise variert: korn, frukt, grønnsaker og andre matvarer. Mat og drikke gir energi til lek og læring.",
                "Havregrøt med bær har flere slags råvarer og gir kroppen energi.",
                "Ingen enkelt matvare gjør hele jobben. Variasjon over tid er viktig."),
        };

    private static readonly IReadOnlyDictionary<string, string> YoungMethods = new Dictionary<string, string>
    {
        ["Norsk"] = "Se på bildet. Les eller lytt til litt tekst. Pek på et spor. Fortell med egne ord.",
        ["Matematikk"] = "Bygg med ting, tegn prikker eller bruk en tallinje. Fortell hva du gjør. Tell én gang til.",
        ["Engelsk"] = "Look, listen and repeat. Bruk et kjent ord i en liten setning. Prøv igjen sammen med noen.",
        ["Naturfag"] = "Se nøye. Still et spørsmål. Tegn det du ser. Fortell hva som forandret seg.",
        ["Samfunnsfag"] = "Se på stedet eller bildet. Finn mennesker og steder. Spør hva ulike personer trenger.",
        ["KRLE"] = "Se, lytt og spør på en vennlig måte. Fortell hva som er likt og hva som er forskjellig.",
        ["Kunst og håndverk"] = "Se på former og materialer. Lag en liten skisse. Prøv trygt. Vis hva du valgte.",
        ["Musikk"] = "Lytt. Finn pulsen med kroppen. Prøv et kort mønster. Stopp og lytt til gruppen.",
        ["Kroppsøving"] = "Se hvor du skal. Prøv rolig og trygt. Kjenn hva kroppen gjør. Hjelp en medelev.",
        ["Mat og helse"] = "Vask hendene. Se på råvarene. Følg ett trinn om gangen. Smak og rydd sammen.",
    };

    private static readonly IReadOnlyDictionary<string, string> SubjectIcons = new Dictionary<string, string>
    {
        ["Norsk"] = "📖", ["Matematikk"] = "🔢", ["Engelsk"] = "💬", ["Naturfag"] = "🌱",
        ["Samfunnsfag"] = "🏘️", ["KRLE"] = "🤝", ["Kunst og håndverk"] = "🎨", ["Musikk"] = "🎵",
        ["Kroppsøving"] = "🤸", ["Mat og helse"] = "🥣", ["Fremmedspråk"] = "🌍",
        ["Utdanningsvalg"] = "🧭", ["Valgfag"] = "🛠️", ["Arbeidslivsfag"] = "🦺"
    };

    private static readonly IReadOnlyDictionary<(string Subject, string Topic), string[]> YoungTerms =
        new Dictionary<(string, string), string[]>
        {
            [("Norsk", "Leseforståelse")] = ["overskrift", "bilde", "ord", "mening", "fortelle"],
            [("Norsk", "Fortelling og sjanger")] = ["person", "sted", "først", "så", "slutt"],
            [("Matematikk", "Tall og regning")] = ["tall", "telle", "pluss", "minus", "tallinje"],
            [("Matematikk", "Måling og geometri")] = ["sirkel", "trekant", "firkant", "lengde", "centimeter"],
            [("Engelsk", "Everyday English")] = ["hello", "name", "please", "thank you", "answer"],
            [("Engelsk", "Reading and stories")] = ["word", "picture", "character", "first", "next"],
            [("Naturfag", "Kropp og helse")] = ["kropp", "hjerte", "lunger", "puste", "søvn"],
            [("Naturfag", "Dyr, planter og økosystemer")] = ["dyr", "plante", "mat", "levested", "natur"],
            [("Samfunnsfag", "Familie og nærmiljø")] = ["familie", "hjem", "skole", "nærmiljø", "regel"],
            [("Samfunnsfag", "Kart, landskap og ressurser")] = ["kart", "vei", "symbol", "sted", "tegnforklaring"],
            [("KRLE", "Høytider og tradisjoner")] = ["høytid", "tradisjon", "symbol", "familie", "feiring"],
            [("KRLE", "Etikk og vennskap")] = ["venn", "valg", "trygg", "rettferdig", "omsorg"],
            [("Kunst og håndverk", "Farge, form og tegning")] = ["linje", "farge", "sirkel", "form", "tegning"],
            [("Kunst og håndverk", "Materialer og teknikker")] = ["papir", "tre", "stoff", "leire", "verktøy"],
            [("Musikk", "Rytme og puls")] = ["puls", "rytme", "klapp", "pause", "tempo"],
            [("Musikk", "Sang og samspill")] = ["sang", "lytte", "start", "stopp", "sammen"],
            [("Kroppsøving", "Lek og bevegelse")] = ["løpe", "hoppe", "balanse", "kaste", "trygg"],
            [("Kroppsøving", "Fair play og samarbeid")] = ["regel", "tur", "hjelpe", "rettferdig", "lag"],
            [("Mat og helse", "Matglede og kjøkkenhygiene")] = ["håndvask", "ren", "smak", "råvare", "trygg"],
            [("Mat og helse", "Kosthold og næringsstoffer")] = ["mat", "drikke", "energi", "variert", "råvare"]
        };

    public static IReadOnlyList<string> GetTerms(int grade, string subject, AcademicTopic topic) =>
        grade <= 2 && YoungTerms.TryGetValue((subject, topic.Name), out var terms)
            ? terms
            : topic.Terms;

    public static AdaptedTopicText AdaptTopic(int grade, string subject, AcademicTopic topic)
    {
        if (grade <= 2 && YoungTopics.TryGetValue((subject, topic.Name), out var young))
            return grade == 1
                ? young
                : young with { Explanation = $"{young.Explanation} Fagordet «{GetTerms(grade, subject, topic)[0]}» hjelper deg å forklare det du ser." };

        if (grade <= 2)
            return new AdaptedTopicText($"Her lærer vi om {topic.Name.ToLowerInvariant()}.",
                $"Se etter {topic.Terms[0]} og {topic.Terms[1]}. Bruk et bilde, ting eller en handling for å vise hva ordene betyr.",
                $"Et eksempel på {topic.Name.ToLowerInvariant()} kan vise både {topic.Terms[0]} og {topic.Terms[1]}.",
                "Se nøye og forklar det du faktisk kan vise. Ikke velg bare fordi et ord ser kjent ut.");

        return new AdaptedTopicText(topic.Core, topic.Explanation, topic.Example, topic.Misconception);
    }

    public static LessonPedagogy Create(int grade, string subject, AcademicTopic topic, string matureMethod)
    {
        var adapted = AdaptTopic(grade, subject, topic);
        var visualHtml = BuildVisuals(grade, subject, topic, adapted);
        return grade switch
        {
            1 => new(adapted,
                $"Se, tegn og prøv deg fram i {topic.Name.ToLowerInvariant()}. Laget for nye skoleelever.",
                $"Du skal kunne se ett viktig tegn på {topic.Name.ToLowerInvariant()}, prøve det selv og fortelle hva du oppdaget.",
                "Se, les og prøv", "Et eksempel du kan se for deg", "Prøv selv – ett steg om gangen",
                "Les eller lytt sammen med en voksen eller medelev. Ta én liten del om gangen og bruk tegning når ordene blir vanskelige.",
                YoungMethods.GetValueOrDefault(subject, "Se. Prøv. Tegn. Fortell hva du oppdaget."),
                ["Se eller lytt.", "Pek, flytt eller prøv.", "Tegn det du oppdaget.", "Fortell med egne ord."],
                $"Lag en tegning om {topic.Name.ToLowerInvariant()}. Bruk ett av fagordene og fortell én ting tegningen viser.",
                "Vis tegningen til noen. Be dem fortelle hva de ser, og gjør én del enda tydeligere.", 4, visualHtml),
            2 => new(adapted,
                $"Utforsk {topic.Name.ToLowerInvariant()} med bilder, korte forklaringer og noe du kan prøve selv.",
                $"Du skal kunne forklare hovedideen med et eksempel og bruke minst to fagord fra {topic.Name.ToLowerInvariant()}.",
                "Se sammenhengen", "Et eksempel fra hverdagen", "Slik undersøker du",
                "Les en liten del, se på tegningen og si innholdet med egne ord før du går videre.",
                YoungMethods.GetValueOrDefault(subject, "Se. Prøv. Tegn. Forklar med egne ord."),
                ["Finn ut hva du skal se etter.", "Prøv med ting, bilde eller bevegelse.", "Tegn eller noter det som skjedde.", "Kontroller og forklar."],
                $"Lag et nytt eksempel på {topic.Name.ToLowerInvariant()}. Tegn det, og skriv eller fortell to setninger om hva som skjer.",
                "Sammenlign med en medelev. Legg til ett fagord som gjør forklaringen tydeligere.", 5, visualHtml),
            <= 4 => new(adapted,
                $"Lær {topic.Name.ToLowerInvariant()} gjennom tydelige modeller, fagord og eksempler fra hverdagen.",
                $"Du skal kunne beskrive hovedideen, bruke sentrale fagord og vise sammenhengen i et eget eksempel på nivå med {grade}. trinn.",
                "Forklaring og modell", "Eksempel med forklaring", "Arbeid steg for steg",
                "Stopp etter hvert avsnitt. Bruk modellen til å gjenfortelle innholdet, og lag deretter et eget eksempel.",
                matureMethod,
                ["Finn spørsmålet og viktige opplysninger.", "Velg en modell eller metode.", "Vis hvordan du tenker.", "Kontroller svaret i situasjonen."],
                "Lag ett eksempel som passer og ett som ikke passer. Marker detaljen som skiller dem.",
                "Forklar eksemplene til en medelev og forbedre ett sted som var vanskelig å forstå.", 6, visualHtml),
            <= 7 => new(adapted,
                $"Undersøk {topic.Name.ToLowerInvariant()} med faglige modeller, årsakssammenhenger og begrunnede eksempler.",
                $"Du skal kunne forklare hovedideen, bruke presise fagbegreper, anvende en relevant metode og begrunne en konklusjon på nivå med {grade}. trinn.",
                "Faglig forklaring", "Gjennomarbeidet eksempel", "Slik arbeider du faglig",
                "Noter årsaker, virkninger og fagbegreper. Bruk modellen til å kontrollere om forklaringen henger sammen.",
                matureMethod,
                ["Avgrens hva du skal finne eller forklare.", "Velg relevante begreper og metode.", "Vis mellomsteg, observasjoner eller tekstbevis.", "Kontroller resultatet og begrunn konklusjonen."],
                "Bruk arbeidsmåten på en ny situasjon og begrunn løsningen med minst tre nøkkelbegreper.",
                "Be om respons på sammenhengen mellom belegg og konklusjon, og revider forklaringen.", 6, visualHtml),
            _ => new(adapted,
                $"Analyser {topic.Name.ToLowerInvariant()} med presist fagspråk, dokumentasjon og kritisk vurdering.",
                $"Du skal kunne redegjøre for hovedideen, anvende fagets metode selvstendig, vurdere premisser og dokumentasjon og formulere en nyansert konklusjon på nivå med {grade}. trinn.",
                "Faglig rammeverk", "Analyse av et eksempel", "Metode og etterprøvbarhet",
                "Identifiser premisser, dokumentasjon og alternative forklaringer. Skill mellom observasjon, tolkning og konklusjon.",
                matureMethod,
                ["Avgrens problemstillingen og definer sentrale begreper.", "Velg metode og vurder datagrunnlag eller kilder.", "Dokumenter resonnementet og drøft alternative forklaringer.", "Vurder usikkerhet, motargument og etterprøvbarhet før konklusjonen."],
                "Analyser en ny situasjon. Drøft minst én relevant innvending og forklar hvordan konklusjonen kan etterprøves.",
                "Be om kritisk respons på premisser, belegg og presisjon. Revider der responsen avdekker et faktisk hull.", 6, visualHtml)
        };
    }

    private static string BuildVisuals(int grade, string subject, AcademicTopic topic, AdaptedTopicText adapted)
    {
        var icon = SubjectIcons.GetValueOrDefault(subject, "💡");
        var terms = GetTerms(grade, subject, topic);
        var first = E(terms[0]);
        var second = E(terms[1]);
        var third = E(terms[2]);
        var title = E(topic.Name);
        var level = grade == 1 ? 4 : grade == 2 ? 3 : grade <= 4 ? 2 : 1;
        var pictureCards = grade <= 2
            ? $"""
                <div class="visual-aid picture-story" data-visual-aid="picture-cards">
                  <div class="picture-card"><span aria-hidden="true">👀</span><strong>Se</strong><small>Finn {first}</small></div>
                  <div class="picture-arrow" aria-hidden="true">→</div>
                  <div class="picture-card"><span aria-hidden="true">{icon}</span><strong>Prøv</strong><small>Bruk {second}</small></div>
                  <div class="picture-arrow" aria-hidden="true">→</div>
                  <div class="picture-card"><span aria-hidden="true">💬</span><strong>Fortell</strong><small>Si hva du fant</small></div>
                </div>
                """
            : string.Empty;
        var activity = grade == 1
            ? $"<div class=\"visual-aid draw-prompt\" data-visual-aid=\"draw\"><span aria-hidden=\"true\">✏️</span><div><strong>Tegn sammenhengen</strong><p>Tegn {first} og {second}. Bruk en pil for å vise hva som skjer.</p></div></div>"
            : grade == 2
                ? $"<div class=\"visual-aid draw-prompt\" data-visual-aid=\"act\"><span aria-hidden=\"true\">👐</span><div><strong>Gjør modellen levende</strong><p>Bruk ting, kropp eller en tegning til å vise {first}, {second} og {third}.</p></div></div>"
                : grade <= 4
                    ? $"<div class=\"visual-aid compare-model\" data-visual-aid=\"compare\"><div><strong>Eksempel</strong><span>{first} + {second}</span></div><div><strong>Ikke-eksempel</strong><span>Mangler sammenheng eller belegg</span></div></div>"
                    : string.Empty;
        var diagramCaption = grade <= 2 ? "Se hvordan delene kan henge sammen." : grade <= 7
            ? "Bruk modellen til å forklare en faglig sammenheng." : "Skill premiss, belegg og konklusjon i analysen.";
        var diagramLabels = grade <= 4
            ? (first, second, third)
            : grade <= 7 ? ("Observasjon", "Fagbegrep", "Forklaring") : ("Premiss", "Dokumentasjon", "Konklusjon");
        var diagram = $"""
            <figure class="visual-aid concept-drawing" data-visual-aid="diagram">
              <svg viewBox="0 0 760 190" role="img" aria-labelledby="visual-title-{grade}-{SafeId(subject)}-{SafeId(topic.Name)}">
                <title id="visual-title-{grade}-{SafeId(subject)}-{SafeId(topic.Name)}">Visuell modell for {title}</title>
                <defs><marker id="arrow-{grade}-{SafeId(subject)}-{SafeId(topic.Name)}" markerWidth="10" markerHeight="10" refX="8" refY="3" orient="auto"><path d="M0,0 L0,6 L9,3 z" /></marker></defs>
                <rect x="18" y="48" width="200" height="90" rx="24" class="visual-node node-one"/><text x="118" y="88" text-anchor="middle">{diagramLabels.Item1}</text><text x="118" y="113" text-anchor="middle" class="node-icon">{icon}</text>
                <path d="M225 93 H270" class="visual-arrow" marker-end="url(#arrow-{grade}-{SafeId(subject)}-{SafeId(topic.Name)})"/>
                <rect x="280" y="48" width="200" height="90" rx="24" class="visual-node node-two"/><text x="380" y="88" text-anchor="middle">{diagramLabels.Item2}</text><text x="380" y="114" text-anchor="middle" class="node-icon">◆</text>
                <path d="M487 93 H532" class="visual-arrow" marker-end="url(#arrow-{grade}-{SafeId(subject)}-{SafeId(topic.Name)})"/>
                <rect x="542" y="48" width="200" height="90" rx="24" class="visual-node node-three"/><text x="642" y="88" text-anchor="middle">{diagramLabels.Item3}</text><text x="642" y="114" text-anchor="middle" class="node-icon">✓</text>
              </svg>
              <figcaption>{E(diagramCaption)}</figcaption>
            </figure>
            """;
        return $"""
            <section class="visual-learning visual-level-{level}" data-visual-level="{level}" aria-label="Visuell forklaring">
              <div class="visual-heading"><span aria-hidden="true">{icon}</span><div><h3>{(grade <= 2 ? "Se det for deg" : grade <= 4 ? "Bygg en modell" : "Faglig modell")}</h3><p>{(grade <= 2 ? $"Bilder og piler hjelper deg å forstå {title}." : $"Les modellen fra venstre mot høyre og knytt den til {title}.")}</p></div></div>
              {pictureCards}
              {diagram}
              {activity}
            </section>
            """;
    }

    private static string SafeId(string value) => new(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
    private static string E(string value) => WebUtility.HtmlEncode(value);
}
