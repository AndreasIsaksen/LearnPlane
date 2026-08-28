using LearnPlane.Web.Models;

namespace LearnPlane.Web.Data;

internal static class AcademicQuestionCatalog
{
    public static ICollection<QuizQuestion> Build(int grade, string subject, AcademicTopic topic,
        AcademicTopic[] subjectTopics, int courseNumber) => subject switch
        {
            "Matematikk" => BuildMathematics(grade, topic.Name, courseNumber),
            "Engelsk" => BuildEnglish(grade, topic.Name, courseNumber),
            "Norsk" => BuildNorwegian(grade, topic.Name, courseNumber),
            _ => BuildAppliedSubjectQuestions(grade, subject, topic, subjectTopics, courseNumber)
        };

    private static ICollection<QuizQuestion> BuildMathematics(int grade, string topic, int variant)
    {
        var questions = new List<QuestionSeed>();
        switch (topic)
        {
            case "Tall og regning" when grade <= 2:
                var addA = 7 + grade * 3;
                var addB = 4 + grade;
                var start = 20 + grade * 5;
                var taken = 6 + grade;
                questions.Add(Q($"Hva er {addA} + {addB}?", addA + addB, addA + addB - 1, addA + addB + 2, addA - addB,
                    $"Del opp {addB} og tell videre fra {addA}; summen blir {addA + addB}."));
                questions.Add(Q($"Det ligger {start} klosser i en kasse. {taken} tas ut. Hvor mange er igjen?", start - taken, start + taken, taken, start - taken + 1,
                    $"«Tas ut» betyr subtraksjon: {start} − {taken} = {start - taken}."));
                questions.Add(Q("Hvilket tall har 3 tiere og 6 enere?", 36, 63, 306, 9,
                    "Tre tiere er 30 og seks enere er 6; 30 + 6 = 36."));
                questions.Add(Q($"Hvilket regnestykke kan kontrollere at {start} − {taken} = {start - taken}?",
                    $"{start - taken} + {taken} = {start}", $"{start} + {taken} = {start + taken}", $"{taken} − {start - taken} = {taken - (start - taken)}", $"{start - taken} − {taken} = {start - 2 * taken}",
                    "Addisjon kontrollerer subtraksjon fordi regneartene er motsatte."));
                break;

            case "Måling og geometri" when grade <= 2:
                var firstLength = 5 + grade;
                var secondLength = 3 + grade;
                questions.Add(Q("Hvilken figur har nøyaktig tre rette sider og tre hjørner?", "En trekant", "En sirkel", "Et rektangel", "En kule", "En trekant har tre sider og tre hjørner."));
                questions.Add(Q($"Et bånd er {firstLength} cm langt, og et annet er {secondLength} cm. Hvor lange er de til sammen?", $"{firstLength + secondLength} cm", $"{firstLength - secondLength} cm", $"{firstLength + secondLength} m", $"{firstLength * secondLength} cm", $"Lengdene adderes: {firstLength} cm + {secondLength} cm = {firstLength + secondLength} cm."));
                questions.Add(Q("Hvilket utsagn om en sirkel er riktig?", "Den har ingen hjørner", "Den har fire like sider", "Den har tre hjørner", "Den er alltid en kule", "En sirkel er en flat figur med krum kant og uten hjørner."));
                questions.Add(Q("Hvilken enhet passer best når du måler lengden på en blyant?", "Centimeter", "Liter", "Kilogram", "Grader", "Lengde på små ting måles vanligvis i centimeter."));
                break;

            case "Måling og geometri":
                var width = grade + 3;
                var height = grade;
                questions.Add(Q($"Et rektangel er {width} cm langt og {height} cm bredt. Hva er omkretsen?", $"{2 * (width + height)} cm", $"{width * height} cm²", $"{width + height} cm", $"{2 * width + height} cm", $"Omkretsen er 2 · ({width} + {height}) = {2 * (width + height)} cm."));
                questions.Add(Q($"Hva er arealet av det samme rektangelet på {width} cm × {height} cm?", $"{width * height} cm²", $"{2 * (width + height) + 2} cm²", $"{width + height} cm²", $"{width * height} cm", $"Arealet er lengde · bredde = {width * height} cm²."));
                questions.Add(Q("En vinkel er mindre enn 90°. Hva kalles den?", "Spiss vinkel", "Rett vinkel", "Stump vinkel", "Hel vinkel", "En spiss vinkel er større enn 0° og mindre enn 90°."));
                questions.Add(Q("En figur har én loddrett symmetrilinje. Hva betyr det?", "De to sidene speiler hverandre over linjen", "Figuren har alltid fire sider", "Arealet er lik omkretsen", "Alle vinklene er 90°", "En symmetrilinje deler figuren i speilvendte, sammenfallende deler."));
                break;

            case "Brøk, desimaltall og prosent" when grade <= 4:
                var numerator = grade - 1;
                questions.Add(Q($"En pizza deles i 8 like deler. {numerator} deler spises. Hvilken brøk er spist?", $"{numerator}/8", $"8/{numerator}", $"{8 - numerator}/8", $"{numerator}/7", "Telleren viser spiste deler, og nevneren viser alle de åtte like delene."));
                questions.Add(Q("Hvilke brøker har samme verdi?", "1/2 og 2/4", "1/2 og 1/4", "2/3 og 3/2", "1/3 og 2/3", "Når teller og nevner i 1/2 dobles, får vi den likeverdige brøken 2/4."));
                questions.Add(Q("Hvilken brøk er størst?", "3/4", "1/4", "2/4", "1/8", "Når firedeler sammenlignes, er tre deler mer enn to og én; 3/4 er også større enn 1/8."));
                questions.Add(Q("Hvilket desimaltall er det samme som en halv?", "0,5", "0,2", "1,5", "0,05", "En halv er fem tideler, altså 0,5."));
                break;

            case "Brøk, desimaltall og prosent":
                var percentage = 10 * (grade - 3);
                var whole = 200;
                questions.Add(Q($"Hva er {percentage} % av {whole}?", percentage * whole / 100, percentage + whole, whole - percentage, whole / percentage, $"{percentage} % = {percentage / 100m:0.0#}. Gang med {whole}: svaret er {percentage * whole / 100}."));
                questions.Add(Q("Hvilken verdi er størst?", "0,8", "3/4", "70 %", "0,65", "Skriv alle som desimaltall: 0,8; 0,75; 0,70; 0,65."));
                questions.Add(Q("En pris øker fra 400 kr til 500 kr. Hvor stor er prosentøkningen?", "25 %", "20 %", "100 %", "10 %", "Økningen er 100 kr. 100/400 = 0,25 = 25 %."));
                questions.Add(Q("Hva er 2/3 + 1/6?", "5/6", "3/9", "3/6", "2/9", "Gjør om 2/3 til 4/6. Da blir 4/6 + 1/6 = 5/6."));
                break;

            case "Algebra og likninger" when grade <= 7:
                var solution = grade + 2;
                var constant = 4 + variant;
                var total = 3 * solution + constant;
                questions.Add(Q($"Løs likningen 3x + {constant} = {total}.", $"x = {solution}", $"x = {solution + constant}", $"x = {total - constant}", $"x = {solution - 1}", $"Trekk fra {constant} og del på 3: x = {solution}."));
                questions.Add(Q($"Mønsteret er {grade}, {grade + 3}, {grade + 6}, {grade + 9}, … Hva er neste tall?", grade + 12, grade + 10, grade + 11, grade + 15, "Forskjellen mellom nabotallene er alltid 3."));
                questions.Add(Q("Hvilket uttrykk betyr «fem mer enn det dobbelte av n»?", "2n + 5", "2(n + 5)", "5n + 2", "n + 7", "Det dobbelte er 2n; fem mer gir 2n + 5."));
                questions.Add(Q("Forenkle 4a + 3 + 2a − 1.", "6a + 2", "6a + 4", "8a + 2", "4a + 4", "Samle a-leddene og konstantleddene hver for seg: 4a + 2a = 6a og 3 − 1 = 2."));
                break;

            case "Algebra og likninger":
                var x = grade - 3;
                var c = grade + 1;
                var rhs = 2 * x + c;
                questions.Add(Q($"Løs 2x + {c} = {rhs}.", $"x = {x}", $"x = {rhs - c}", $"x = {x + 1}", $"x = {c}", $"Trekk fra {c} på begge sider og del på 2: x = {x}."));
                questions.Add(Q("En lineær funksjon er y = 3x − 2. Hva er y når x = 4?", "10", "14", "5", "9", "Sett inn x = 4: y = 3 · 4 − 2 = 10."));
                questions.Add(Q("Hvilket uttrykk er faktoriseringen av 6x + 9?", "3(2x + 3)", "6(x + 9)", "3(2x + 9)", "9(6x + 1)", "Største felles faktor er 3: 6x/3 = 2x og 9/3 = 3."));
                questions.Add(Q("Linjen går gjennom (0, 2) og (3, 8). Hva er stigningstallet?", "2", "3", "6", "10/3", "Stigningstallet er endring i y delt på endring i x: (8 − 2)/(3 − 0) = 2."));
                break;

            case "Statistikk og sannsynlighet":
                var values = new[] { 2, 4, 4, 5, 5, 5, 8 };
                questions.Add(Q($"Hva er medianen i datasettet {string.Join(", ", values)}?", "5", "4", "33/7", "8", "Det er sju sorterte verdier; den fjerde og midterste er 5."));
                questions.Add(Q($"Hva er typetallet i datasettet {string.Join(", ", values)}?", "5", "4", "2", "8", "Tallet 5 forekommer tre ganger, oftere enn de andre."));
                questions.Add(Q("En pose har 3 røde og 7 blå kuler. Hva er sannsynligheten for rød ved tilfeldig trekk?", "3/10", "3/7", "7/10", "1/3", "Det er 3 gunstige utfall av totalt 10 kuler: 3/10."));
                questions.Add(Q("Et søylediagram starter y-aksen på 95 i stedet for 0. Hva bør leseren være særlig oppmerksom på?", "Små forskjeller kan se svært store ut", "Medianen blir alltid feil", "Søylene viser sannsynlighet", "Datasettet får flere observasjoner", "En avkuttet akse kan visuelt overdrive forskjellene."));
                break;
        }
        return MakeQuestions(questions, grade, variant);
    }

    private static ICollection<QuizQuestion> BuildEnglish(int grade, string topic, int variant)
    {
        var seeds = topic switch
        {
            "Everyday English" => new[]
            {
                Q("Nora says: “Hello! My name is Nora.” Which reply keeps the conversation going?", "Hi Nora! I’m Sam. Nice to meet you.", "Yesterday blue table.", "No, I name.", "Goodbye because apple.", "A greeting and introduction answer Nora and continue the exchange."),
                Q("Your classmate says: “Could you help me, please?” What is the most useful answer?", "Yes, of course. What do you need?", "I am ten years old.", "The book was yesterday.", "Please is a colour.", "The answer responds politely and asks for the information needed."),
                Q("Which sentence asks someone to repeat politely?", "Could you say that again, please?", "Say now!", "You repeat yesterday?", "I don't word.", "“Could you …, please?” is a clear and polite request."),
                Q("Choose the sentence with natural English word order.", "I play football after school.", "I football after school play.", "Play I after football school.", "After I school football.", "A basic statement normally follows subject + verb + object."),
            },
            "Reading and stories" => new[]
            {
                Q("Read: “Ava packed a torch and pulled on her boots. Outside, the sky was already dark.” Where is Ava probably going?", "Outside in the dark", "To bed in bright sunlight", "Swimming without equipment", "Into a classroom at noon", "The torch, boots and dark sky are clues that she is going outside at night."),
                Q("Read: “Ben fed the dog before he left. It wagged its tail.” What does “it” refer to?", "The dog", "Ben", "The food", "The door", "The pronoun “it” replaces the nearest suitable singular noun, “the dog”."),
                Q("Read: “First we mixed the flour. Next we added milk. Finally we baked the batter.” Which word marks the last step?", "Finally", "First", "Next", "Flour", "“Finally” signals the last event in a sequence."),
                Q("Read: “The path was icy, so Mia walked slowly and held the rail.” Why did Mia walk slowly?", "Because the path was slippery", "Because the rail was new", "Because she wanted to run", "Because the path was warm", "“Icy” and “held the rail” provide evidence that the path was slippery."),
            },
            "Grammar and writing" => new[]
            {
                Q("Yesterday, Amir ___ to the library and borrowed two books.", "went", "go", "goes", "going", "“Yesterday” requires past tense; the irregular past of “go” is “went”."),
                Q("Choose the sentence with correct subject–verb agreement.", "My sister plays the guitar every day.", "My sister play the guitar every day.", "My sister playing the guitar every day.", "My sister are play the guitar.", "A third-person singular subject takes “plays” in the present simple."),
                Q("Which sentence connects a result to its reason most clearly?", "We stayed inside because the storm was strong.", "We stayed inside but the storm was strong because.", "Because we stayed the storm inside.", "The storm and inside stayed.", "“Because” introduces the reason for staying inside."),
                Q("Lina and Sara finished the project. Which pronoun can replace “Lina and Sara”?", "They", "She", "It", "He", "Two people are replaced by the plural pronoun “they”."),
            },
            "English-speaking cultures" => new[]
            {
                Q("A video shows one family celebrating a holiday in Canada. Which conclusion is most accurate?", "It shows one real example, not how every Canadian family celebrates.", "All Canadian families celebrate exactly this way.", "The video proves Canada has only one culture.", "No conclusion can ever be drawn from a video.", "A source can document a particular experience without representing an entire country."),
                Q("A British speaker says “football” and an American speaker says “soccer” about the same sport. What does this show?", "English vocabulary varies with context and place.", "One speaker does not know English.", "The sports must be different everywhere.", "Accents determine the rules of sport.", "Both words are established choices in their language varieties."),
                Q("Which research question avoids a broad stereotype?", "How do three teenagers from different parts of Ireland describe school life?", "What are all Irish people like?", "Why does everyone in Ireland think the same?", "Which single custom defines every English speaker?", "The first question names a limited group and allows varied perspectives."),
                Q("Two reliable sources disagree about a cultural practice. What should you do next?", "Check their time, place, purpose and evidence before comparing them.", "Keep only the source you saw first.", "Assume the shortest source is correct.", "Conclude that facts never matter.", "Source context can explain why descriptions differ."),
            },
            _ => new[]
            {
                Q("Which sentence introduces a counterargument respectfully?", "Some may disagree; however, the data indicate a different trend.", "You are wrong and I will not listen.", "My point needs no evidence.", "Everyone agrees with me.", "It acknowledges another position and then connects a response to evidence."),
                Q("Choose the best signpost for moving from the problem to a possible solution.", "Having examined the problem, let us consider two solutions.", "Blue quickly perhaps.", "The solution before problem yesterday.", "I have no structure.", "The phrase clearly tells the audience how the presentation is moving forward."),
                Q("Which claim is supported by relevant evidence?", "The library should open later because the usage log shows 60% of visits occur after 15:00.", "The library should open later because I said so.", "The library is best; everybody knows it.", "Opening hours and weather are always identical.", "The usage data directly support the proposal about opening hours."),
                Q("You do not understand a speaker's last point. What is the best response?", "Could you clarify what you mean by the final example?", "I will pretend I understood.", "Speak faster without a question.", "Change the topic immediately.", "A focused clarification question repairs understanding and advances discussion."),
            }
        };
        return MakeQuestions(seeds, grade, variant);
    }

    private static ICollection<QuizQuestion> BuildNorwegian(int grade, string topic, int variant)
    {
        var seeds = topic switch
        {
            "Leseforståelse" => new[]
            {
                Q("Les: «Lina pakket regnjakke og støvler. Ute trommet det mot ruta.» Hvordan er været?", "Det regner", "Det snør tett", "Det er skyfritt", "Det er svært varmt", "Regnjakke, støvler og lyden mot ruta er tekstbevis for regn."),
                Q("Les: «Først fant Omar fram mel. Deretter knakk han egg i bollen.» Hva gjorde Omar først?", "Han fant fram mel", "Han knakk egg", "Han satte bollen i ovnen", "Han spiste maten", "Tidsordet «først» markerer den første handlingen."),
                Q("Les: «Parken har nye husker, en bred sklie og benker. Nå kan flere leke og hvile der.» Hva er hovedideen?", "Parken er forbedret for ulike brukere", "Alle må bruke sklien", "Benkene skal fjernes", "Parken er stengt", "Begge setningene handler om nye tilbud som gjør parken nyttig for flere."),
                Q("Hvilket svar bruker tekstbevis best?", "Jeg tror hun fryser, fordi teksten sier at hun skjelver og trekker jakken tett rundt seg.", "Jeg tror hun fryser, uten noen grunn.", "Hun fryser fordi alle alltid fryser.", "Teksten betyr det jeg ønsker.", "Et godt tolkningssvar viser til konkrete opplysninger i teksten."),
            },
            "Fortelling og sjanger" => new[]
            {
                Q("«Det var en gang tre bukker som skulle over en bro.» Hvilke trekk peker tydeligst mot eventyr?", "Fast åpningsformel og tallet tre", "Fotnoter og statistikk", "Dato og avsender", "Bruksanvisning i nummererte trinn", "Formelen «det var en gang» og tretallet er vanlige eventyrtrekk."),
                Q("Hva er konflikten i en fortelling?", "Problemet eller motsetningen som driver handlingen", "Listen over alle adjektiv", "Navnet på forlaget", "Antall sider i boka", "Konflikten skaper spenning og tvinger fram valg eller forandring."),
                Q("En historie begynner midt i en biljakt og forklarer bakgrunnen senere. Hvilket virkemiddel brukes?", "In medias res", "Alfabetisk register", "Enderim", "Kildehenvisning", "In medias res betyr at fortellingen starter midt i handlingen."),
                Q("Hvilket tekststed er personskildring?", "«Ada unngikk blikket deres, men stilte seg likevel foran døra.»", "«Boka har 96 sider.»", "«Toget går klokken 14.10.»", "«Bland 2 dl vann og mel.»", "Handling og kroppsspråk viser både usikkerhet og mot hos Ada."),
            },
            "Rettskriving og grammatikk" => new[]
            {
                Q("Hvilket ord er verbet i setningen «Den lille hunden bjeffer høyt»?", "bjeffer", "hunden", "lille", "høyt", "Verbet «bjeffer» forteller hva hunden gjør."),
                Q("Hvilken setning har riktig tegnsetting?", "Etter skolen gikk Iben hjem, spiste middag og gjorde lekser.", "etter skolen gikk Iben hjem spiste middag og gjorde lekser", "Etter skolen, gikk iben hjem spiste middag.", "Etter skolen gikk Iben hjem spiste, middag og gjorde lekser", "Setningen starter med stor bokstav, har egennavn med stor bokstav, oppramsingskomma og punktum."),
                Q("Finn subjekt og verbal i «Fuglene bygger rede i treet». ", "Subjekt: Fuglene. Verbal: bygger.", "Subjekt: rede. Verbal: treet.", "Subjekt: bygger. Verbal: Fuglene.", "Subjekt: i. Verbal: rede.", "Fuglene utfører handlingen, og bygger uttrykker handlingen."),
                Q("Hvilken setning bruker adjektivet til å gjøre beskrivelsen mer presis?", "Den rustne sykkelen knirket i bakken.", "Sykkelen være knirke.", "Rust sykkel bakken den.", "Sykkelen og og bakken.", "«Rustne» beskriver sykkelen og gir leseren konkret informasjon."),
            },
            "Kildekritikk og argumentasjon" => new[]
            {
                Q("En anonym konto hevder at en energidrikk dobler konsentrasjonen. Hva bør undersøkes først?", "Avsender, dokumentasjon og om uavhengige kilder støtter påstanden", "Hvor mange emojier innlegget har", "Om innlegget har sterke farger", "Om en venn allerede delte det", "Troverdighet vurderes gjennom opphav, belegg og uavhengig kontroll."),
                Q("Hvilket argument støtter påstanden «skolen bør ha flere sykkelstativer» best?", "En telling viser at 34 sykler står ulåst utenfor stativene hver dag.", "Sykkelstativer er et langt ord.", "Jeg liker blå sykler.", "Alle vet at mitt forslag er best.", "Tellingen er relevant dokumentasjon på et konkret kapasitetsbehov."),
                Q("Hva er et motargument?", "En begrunnet innvending mot en påstand", "En gjentakelse av samme påstand", "Navnet på kilden", "Et bilde uten sammenheng", "Et motargument utfordrer påstanden med en relevant grunn."),
                Q("To nettsider bruker nøyaktig samme feilaktige tekst. Hvorfor er de ikke nødvendigvis to uavhengige kilder?", "Den ene kan ha kopiert den andre eller begge kan ha samme opphav.", "Lik tekst beviser alltid sannheten.", "Nettsider kan aldri brukes som kilder.", "Feil blir fakta når den gjentas.", "Uavhengighet handler om informasjonsopphav, ikke bare antall nettsider."),
            },
            _ => new[]
            {
                Q("Hvilket utsagn om norske dialekter er faglig riktig?", "Dialekter har systematiske mønstre for ord, bøying og uttale.", "Dialekter er feilskrevet bokmål.", "Bare én dialekt har grammatikk.", "Dialekter brukes aldri i offentlige sammenhenger.", "Dialekter er fullverdige talemålsvarianter med egne mønstre."),
                Q("En elev skriver bokmål i en rapport og snakker dialekt i pausen. Hva viser dette?", "Språkbruk tilpasses situasjon uten at identiteten må være inkonsekvent.", "Eleven har glemt begge språk.", "Dialekten kan bare brukes skriftlig.", "Bokmål er en dialekt.", "Mennesker kan bruke flere språklige varieteter etter formål og mottaker."),
                Q("Hva er den mest presise forskjellen på bokmål og nynorsk?", "De er to norske skriftspråk med ulike normer.", "De er to dialekter fra Oslo.", "Nynorsk er bare gammelt bokmål.", "Bokmål er talespråket til alle nordmenn.", "Bokmål og nynorsk er normerte skriftspråk, mens talemål varierer i dialekter."),
                Q("Hvorfor kan kodeveksling være nyttig?", "En språkbruker kan tilpasse språk til mottaker, tema eller fellesskap.", "Den fjerner all språklig identitet.", "Den gjør at ord mister betydning.", "Den er alltid et tegn på manglende språkferdighet.", "Kodeveksling kan være en avansert og meningsfull tilpasning."),
            }
        };
        return MakeQuestions(seeds, grade, variant);
    }

    private static ICollection<QuizQuestion> BuildAppliedSubjectQuestions(int grade, string subject,
        AcademicTopic topic, AcademicTopic[] topics, int variant)
    {
        var others = topics.Where(x => x != topic).ToArray();
        var offset = (grade + variant) % others.Length;
        AcademicTopic O(int index) => others[(offset + index) % others.Length];
        var prompts = new[]
        {
            $"Hvilken forklaring av «{topic.Name.ToLowerInvariant()}» er faglig mest presis?",
            $"En elev på {grade}. trinn skal forklare «{topic.Name.ToLowerInvariant()}». Hvilket utsagn bør eleven bruke?",
            $"Hvilket utsagn viser korrekt forståelse av temaet «{topic.Name}»?",
            $"Velg den faglig holdbare forklaringen som hører til «{topic.Name}»."
        };
        var examplePrompts = new[]
        {
            $"Les situasjonen: {topic.Example} Hva er den viktigste faglige sammenhengen?",
            $"Hva viser dette eksemplet fra {subject.ToLowerInvariant()}? {topic.Example}",
            $"Hvilken kjerneidé brukes i følgende situasjon? {topic.Example}",
            $"Hvordan bør eksemplet tolkes faglig? {topic.Example}"
        };
        var seeds = new[]
        {
            Q(prompts[(grade + variant) % prompts.Length], topic.Core, O(0).Core, O(1).Core, O(2).Core,
                $"Kjerneideen er: {topic.Core}"),
            Q(examplePrompts[(grade + variant * 2) % examplePrompts.Length], topic.Core, O(1).Core, O(2).Core, O(0).Core,
                $"Opplysningene i eksemplet anvender nettopp denne sammenhengen: {topic.Core}"),
            Q($"En medelev hevder: «{topic.Misconception}» Hvilken respons retter misforståelsen best?",
                $"Påstanden må korrigeres: {topic.Core}", $"Påstanden er alltid riktig uten forbehold.", O(0).Core, O(1).Core,
                $"Den faglige korrigeringen bygger på kjerneideen: {topic.Core}"),
            Q($"Hvilken samling inneholder bare nøkkelbegreper fra «{topic.Name}»?",
                string.Join(" · ", topic.Terms.Take(3)), string.Join(" · ", O(0).Terms.Take(3)),
                string.Join(" · ", O(1).Terms.Take(3)), string.Join(" · ", O(2).Terms.Take(3)),
                $"{string.Join(", ", topic.Terms.Take(3))} brukes alle direkte i dette temaet.")
        };
        return MakeQuestions(seeds, grade, variant);
    }

    private static ICollection<QuizQuestion> MakeQuestions(IEnumerable<QuestionSeed> seeds, int grade, int variant)
    {
        var openings = new[] { "Løs oppgaven:", "Tenk faglig:", "Bruk det du har lært:", "Vis forståelse:" };
        return seeds.Select((seed, index) => Question(index + 1,
            seed with { Text = $"{openings[index]} {seed.Text} Oppgaven er tilpasset {grade}. trinn." },
            (grade + variant + index) % 4)).ToList();
    }

    private static QuizQuestion Question(int sortOrder, QuestionSeed seed, int correctPosition)
    {
        var options = new List<(string Text, bool Correct)>
        {
            (seed.Wrong1, false), (seed.Wrong2, false), (seed.Wrong3, false)
        };
        options.Insert(correctPosition, (seed.Correct, true));
        return new QuizQuestion
        {
            SortOrder = sortOrder, Text = seed.Text, Explanation = seed.Explanation,
            Options = options.Select((option, index) => new AnswerOption
            {
                Text = option.Text, IsCorrect = option.Correct, SortOrder = index + 1
            }).ToList()
        };
    }

    private static QuestionSeed Q(string text, object correct, object wrong1, object wrong2, object wrong3,
        string explanation) => new(text, $"{correct}", $"{wrong1}", $"{wrong2}", $"{wrong3}", explanation);

    private sealed record QuestionSeed(string Text, string Correct, string Wrong1, string Wrong2, string Wrong3,
        string Explanation);
}
