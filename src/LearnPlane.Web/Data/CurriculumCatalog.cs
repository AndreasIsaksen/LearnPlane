using System.Net;
using LearnPlane.Web.Models;

namespace LearnPlane.Web.Data;

internal sealed record AcademicTopic(string Name, string Core, string Explanation, string Example,
    string Misconception, string[] Terms);

public sealed record GamePair(string Prompt, string Answer);
public sealed record GameVocabulary(string Intro, IReadOnlyList<string> Targets, IReadOnlyList<string> Distractors,
    IReadOnlyList<GamePair> Pairs, IReadOnlyList<string> SequencePieces);

public static class CurriculumCatalog
{
    public const string ContentVersion = "academic-v4";

    private static AcademicTopic T(string name, string core, string explanation, string example,
        string misconception, params string[] terms) => new(name, core, explanation, example, misconception, terms);

    private static readonly IReadOnlyDictionary<string, AcademicTopic[]> Topics =
        new Dictionary<string, AcademicTopic[]>
        {
            ["Norsk"] =
            [
                T("Leseforståelse", "En aktiv leser finner både direkte informasjon og mening som må tolkes.", "Overskrift, avsnitt, nøkkelord og illustrasjoner gir ledetråder. En slutning kombinerer slike tekstspor med det leseren vet fra før, uten å dikte noe teksten ikke støtter.", "«Mina tok på støvler før hun gikk ut. Da hun kom inn, dryppet det fra jakken.» Tekstsporene støtter slutningen at det regner, selv om ordet regn ikke står der.", "En god leser skal ikke bare huske ord; forståelse krever sammenheng, formål og tekstbevis.", "hovedidé", "nøkkelord", "tekstbevis", "slutning", "avsnitt", "sammendrag", "forfatterens formål", "lesestrategi", "sammenheng", "tolkning"),
                T("Fortelling og sjanger", "Sjanger er mønstre som viser hvordan en tekst er bygd og hvorfor den er skrevet.", "Fortellinger utvikler personer, miljø og handling gjennom konflikt og forandring. Eventyr, noveller, faktatekster og dikt bruker ulike komposisjoner og virkemidler.", "«Det var en gang» og tallet tre peker ofte mot eventyr. En novelle kan begynne midt i handlingen og ha en åpen slutt som leseren må tolke.", "Ikke alle fortellinger må ende lykkelig; en åpen eller alvorlig avslutning kan fullføre tekstens tema.", "handling", "konflikt", "forteller", "personskildring", "miljø", "vendepunkt", "sjanger", "virkemiddel", "komposisjon", "tema"),
                T("Rettskriving og grammatikk", "Grammatikk viser hvordan ord får roller og bindes sammen til tydelige setninger.", "Substantiv navngir, verb uttrykker handling eller tilstand, og adjektiv beskriver. En helsetning trenger normalt subjekt og verbal. Bøying og tegnsetting hjelper leseren å forstå.", "I «Den nysgjerrige reven løper raskt» er «reven» subjekt, «løper» verbal og «nysgjerrige» et adjektiv som beskriver reven.", "Lange setninger er ikke automatisk bedre; en god setning er presis, variert og lett å følge.", "substantiv", "verb", "adjektiv", "subjekt", "verbal", "setningsledd", "ordstilling", "bøying", "tegnsetting", "helsetning"),
                T("Kildekritikk og argumentasjon", "Et sterkt argument kobler en påstand til relevante grunner og etterprøvbar dokumentasjon.", "Kildekritikk undersøker avsender, formål, dato og støtte fra uavhengige kilder. Fakta kan kontrolleres, mens meninger uttrykker vurderinger som må begrunnes.", "«Skoleveien bør få gangfelt» blir sterkere med trafikkmålinger og en forklaring på hvordan gangfelt kan redusere risiko enn med bare «jeg synes det».", "En kilde er ikke troverdig bare fordi den ser profesjonell ut eller deles ofte.", "påstand", "argument", "begrunnelse", "dokumentasjon", "avsender", "formål", "troverdighet", "førstehåndskilde", "motargument", "kildehenvisning"),
                T("Språklig mangfold", "Språk varierer med sted, historie, identitet, situasjon og fellesskap.", "Norge har bokmål og nynorsk, samiske språk, nasjonale minoritetsspråk, tegnspråk og mange dialekter. Språkbrukere tilpasser også språket til mottaker og situasjon.", "En elev kan skrive bokmål på skolen, snakke hallingdialekt hjemme og velge mer formelt språk i en jobbsøknad enn i en melding til en venn.", "Dialekt er ikke slurvete uttale av et skriftspråk; dialekter er komplette og systematiske talemålsvarianter.", "bokmål", "nynorsk", "dialekt", "sosiolekt", "samiske språk", "minoritetsspråk", "tegnspråk", "språkidentitet", "kodeveksling", "språkendring")
            ],
            ["Matematikk"] =
            [
                T("Tall og regning", "Tall kan deles opp og kombineres, og regneartene beskriver handlinger som henger sammen.", "Plassverdisystemet gir et siffer verdi etter posisjon. Addisjon og subtraksjon er motsatte operasjoner, slik multiplikasjon og divisjon er. Overslag kontrollerer om svaret er rimelig.", "347 + 186 kan regnes som 347 + 100 + 80 + 6 = 533. Overslaget 350 + 200 ≈ 550 viser riktig størrelsesorden.", "Flere sifre betyr ikke alltid større verdi: 0,75 er større enn 0,605.", "plassverdi", "addisjon", "subtraksjon", "multiplikasjon", "divisjon", "tallinje", "overslag", "regnerekkefølge", "negativt tall", "potens"),
                T("Måling og geometri", "Geometri beskriver form, størrelse og plassering med egenskaper og måleenheter.", "Omkrets måler lengden rundt en figur, areal måler flaten inni, vinkler måles i grader og volum beskriver rommet en figur fyller.", "Et rektangel på 8 cm × 3 cm har omkrets 2·(8+3)=22 cm og areal 8·3=24 cm². Enheten viser hva som er målt.", "Omkrets og areal er ikke det samme; de måler ulike egenskaper og bruker ulike enheter.", "lengde", "omkrets", "areal", "volum", "vinkel", "parallell", "symmetri", "målestokk", "koordinat", "formlikhet"),
                T("Brøk, desimaltall og prosent", "Brøk, desimaltall og prosent representerer deler av en helhet.", "Nevneren viser hvor mange like deler helheten er delt i, telleren hvor mange deler vi har, og prosent betyr hundredeler. Felles verdi gjør representasjonene sammenlignbare.", "3/4 = 0,75 = 75 %. Av 240 elever er 25 % lik 0,25 · 240 = 60 elever.", "Større nevner betyr ikke større brøk; med lik teller er 1/8 mindre enn 1/4.", "teller", "nevner", "likeverdig brøk", "desimaltall", "prosent", "prosentpoeng", "forhold", "helhet", "brøkstrek", "prosentfaktor"),
                T("Algebra og likninger", "Algebra bruker symboler til å beskrive mønstre, sammenhenger og ukjente størrelser.", "En variabel kan skifte verdi. I en likning må samme operasjon gjøres på begge sider for å bevare likheten, og like ledd kan samles.", "3x + 5 = 20 gir 3x = 15 når 5 trekkes fra på begge sider, og x = 5 når begge sider deles på 3.", "Likhetstegnet betyr ikke «nå kommer svaret»; det sier at begge sider har samme verdi.", "variabel", "uttrykk", "likning", "koeffisient", "konstantledd", "like ledd", "formel", "funksjon", "koordinatsystem", "stigningstall"),
                T("Statistikk og sannsynlighet", "Statistikk beskriver data, mens sannsynlighet tallfester hvor mulig en hendelse er.", "Gjennomsnitt, median og typetall viser ulike sider ved et datasett. Diagramvalg påvirker hva som blir synlig. Sannsynlighet går fra 0 til 1, eller 0 til 100 prosent.", "For 2, 3, 3, 4, 13 er medianen 3 og gjennomsnittet 5. Den høye verdien 13 trekker gjennomsnittet opp.", "Et diagram er ikke alltid nøytralt; avkuttet akse kan overdrive forskjeller.", "datasett", "gjennomsnitt", "median", "typetall", "variasjonsbredde", "frekvens", "diagram", "utfallsrom", "sannsynlighet", "representativt utvalg")
            ],
            ["Engelsk"] =
            [
                T("Everyday English", "Meaning grows when familiar words are used in complete, purposeful exchanges.", "Greetings, questions and polite phrases change with the situation. Listening for key words and answering with a full phrase keeps conversation moving even when every word is not understood.", "When someone asks “How are you?”, “I’m fine, thank you. How are you?” answers and invites the other speaker to continue.", "Communication does not fail when one word is unknown; context and follow-up questions can repair understanding.", "greeting", "question", "answer", "please", "thank you", "introduce", "repeat", "listen", "context", "conversation"),
                T("Reading and stories", "Readers combine words, structure and clues to understand events and implied meaning.", "Characters act for reasons, settings shape events and linking words show sequence. A prediction must be based on text evidence and revised when new clues appear.", "“Leo hid the broken vase when he heard the key in the door” suggests that Leo expects trouble; his action is evidence for the inference.", "A prediction is not any guess; it must connect to details in the text.", "character", "setting", "plot", "clue", "inference", "prediction", "sequence", "main idea", "evidence", "summary"),
                T("Grammar and writing", "Grammar connects people, actions and time so readers can follow a message.", "English statements usually use subject–verb–object order. Verbs show tense, pronouns replace nouns and conjunctions connect ideas. Editing improves clarity and accuracy.", "“Yesterday, Maya walked to school because the bus was late” uses past tense and “because” to connect an action with its reason.", "Not every English verb forms the past with -ed; common irregular verbs include go–went and see–saw.", "subject", "verb", "object", "tense", "pronoun", "adjective", "adverb", "conjunction", "paragraph", "revision"),
                T("English-speaking cultures", "English is used in diverse communities, and language reflects history and identity.", "There is no single English-speaking culture or accent. Comparing sources from different places reveals variation without turning differences into stereotypes.", "“Football” names one sport in Britain, while “soccer” distinguishes it from American football in the USA; both choices fit their contexts.", "One accent is not correct while all others are defective; standard and regional varieties follow meaningful patterns.", "variety", "accent", "identity", "community", "tradition", "perspective", "stereotype", "source", "global English", "cultural context"),
                T("Discussion and presentation", "Effective speakers adapt evidence, structure and language to purpose and audience.", "Discussion develops when participants explain claims, listen, ask follow-up questions and respond to evidence. A presentation needs a clear opening, organised points and conclusion.", "“I understand your point; however, the survey suggests…” acknowledges another view before introducing counter-evidence.", "Fluency does not mean speaking quickly; clear pacing, pauses and emphasis usually help the audience more.", "claim", "evidence", "audience", "purpose", "counterargument", "signposting", "formal language", "follow-up question", "delivery", "conclusion")
            ],
            ["Naturfag"] =
            [
                T("Kropp og helse", "Kroppens organsystemer samarbeider, og helse påvirkes av levevaner, miljø og arv.", "Fordøyelsen frigjør næringsstoffer, lungene tar opp oksygen, og blodet frakter stoffene til cellene. Søvn, aktivitet og variert mat støtter kroppens regulering.", "Når du løper, øker pust og puls fordi musklene trenger mer oksygen og raskere transport av stoffer.", "Én matvare eller treningsform kan ikke alene gjøre alle friske; kroppen og livssituasjoner er sammensatte.", "celle", "organ", "fordøyelse", "respirasjon", "blodomløp", "næringsstoff", "immunforsvar", "søvn", "pubertet", "folkehelse"),
                T("Dyr, planter og økosystemer", "Et økosystem består av organismer og fysisk miljø som påvirker hverandre.", "Planter binder solenergi gjennom fotosyntese. Energi og stoff går videre i næringskjeder, og nedbrytere resirkulerer næringsstoffer. Én bestand kan påvirke mange andre.", "Færre insekter kan gi mindre mat til småfugler og mindre pollinering; virkningen sprer seg gjennom næringsnettet.", "Pilen i en næringskjede viser retningen energien overføres, ikke hvem som jakter på hvem.", "art", "habitat", "produsent", "forbruker", "nedbryter", "næringskjede", "næringsnett", "fotosyntese", "biologisk mangfold", "tilpasning"),
                T("Stoff, energi og krefter", "Stoff har målbare egenskaper, energi omformes, og krefter endrer bevegelse eller form.", "Partikkelmodellen forklarer fast stoff, væske og gass. Energi forsvinner ikke, men kan spres som varme. En kraft har retning og størrelse.", "Når en sykkel bremser, omformes bevegelsesenergi hovedsakelig til varme i bremser og dekk gjennom friksjon.", "Energi blir ikke brukt opp og borte; den omformes eller overføres.", "partikkel", "stoff", "faseovergang", "energi", "energikjede", "kraft", "friksjon", "fart", "akselerasjon", "elektrisk krets"),
                T("Jorda, klima og bærekraft", "Jordas systemer samhandler, og bærekraft krever miljømessige, sosiale og økonomiske vurderinger.", "Vær er kortvarige forhold, klima er mønstre over tiår, og drivhusgasser absorberer varmestråling. Tiltak kan ha både gevinster og kostnader.", "Solceller gir fornybar strøm i drift, men krever areal og materialer; en helhetsvurdering ser på produksjon, levetid og gjenbruk.", "En kald dag motbeviser ikke global oppvarming; enkeltdager er vær, klimatrender bygger på lange måleserier.", "vær", "klima", "drivhuseffekt", "karbonkretsløp", "naturressurs", "fornybar", "utslipp", "økologisk fotavtrykk", "bærekraft", "livsløp"),
                T("Vitenskapelig metode", "Naturvitenskapelige påstander må kunne undersøkes systematisk mot data.", "Et godt forsøk har presist spørsmål, testbar hypotese, kontrollerte variabler, gjentatte målinger og en konklusjon som ikke går lenger enn resultatene.", "For å teste lys og plantevekst varieres lysmengden, mens plantetype, jord, vann og tid holdes mest mulig likt.", "Hypotesen må ikke være riktig for at forsøket skal lykkes; en avvist hypotese kan også gi kunnskap.", "problemstilling", "hypotese", "variabel", "kontroll", "måling", "data", "usikkerhet", "gjentakelse", "konklusjon", "etterprøvbarhet")
            ],
            ["Samfunnsfag"] =
            [
                T("Familie og nærmiljø", "Nærmiljøet formes av mennesker, regler, tjenester, steder og ulike behov.", "Familier og lokalsamfunn organiseres på mange måter. Kommunen leverer tjenester, mens innbyggere deltar gjennom arbeid, organisasjoner og demokratiske kanaler.", "Når en park endres, kan barn, naboer, politikere og naturvernere ha ulike interesser. Et godt vedtak lytter til flere grupper.", "Alle i et lokalsamfunn har ikke samme erfaring; livssituasjon og interesser gir ulike perspektiver.", "familie", "nærmiljø", "kommune", "tjeneste", "regel", "tilhørighet", "mangfold", "medvirkning", "interesse", "fellesskap"),
                T("Kart, landskap og ressurser", "Kart er modeller som velger informasjon om sted, avstand, retning og sammenheng.", "Målestokk kobler kartavstand til virkelighet. Naturprosesser og menneskelig bruk former landskap, og ressurser er ulikt fordelt.", "I målestokk 1:50 000 tilsvarer 1 cm 500 meter. En kartavstand på 4 cm er derfor 2 km.", "Et kart er ikke en fullstendig kopi; alle kart forenkler etter formålet.", "kart", "tegnforklaring", "målestokk", "himmelretning", "koordinat", "landskap", "naturressurs", "bosetting", "arealbruk", "bærekraft"),
                T("Historie og kildebruk", "Historisk kunnskap bygges ved å tolke spor fra fortiden i lys av opphav og sammenheng.", "Kilder gir ikke hele fortiden; de må dateres, sammenlignes og undersøkes for perspektiv og formål. En kilde kan være både levning og beretning.", "Et soldatbrev viser skribentens opplevelse, men kan ikke alene fortelle hva alle soldater mente eller alt som hendte.", "En førstehåndskilde er ikke alltid sann og nøytral; øyenvitner kan ta feil eller ha interesser.", "kilde", "levning", "beretning", "førstehåndskilde", "perspektiv", "kontekst", "årsak", "konsekvens", "kontinuitet", "kronologi"),
                T("Demokrati og medborgerskap", "Demokrati kombinerer folkelig innflytelse, rettsstat, rettigheter og mindretallsvern.", "Valg gir representasjon, men demokrati foregår også gjennom debatt, organisasjoner og medvirkning. Makt deles for å begrense misbruk.", "Et flertall kan vedta skatter, men kan ikke uten videre fjerne en minoritets ytringsfrihet; rettigheter begrenser flertallsmakten.", "Demokrati betyr ikke bare at flertallet alltid bestemmer; rettsstat og mindretallsvern er nødvendige.", "demokrati", "valg", "representasjon", "maktfordeling", "rettsstat", "ytringsfrihet", "mindretall", "medborgerskap", "organisasjon", "offentlighet"),
                T("Økonomi og globalisering", "Økonomiske valg fordeler knappe ressurser og knytter husholdninger, bedrifter og land sammen.", "Pris påvirkes av tilbud og etterspørsel. Budsjett viser inntekter og utgifter. Globale verdikjeder skaper muligheter, avhengighet og miljøbelastning.", "En T-skjorte kan ha bomull fra ett land, søm fra et annet og salg i Norge; prisen skjuler ikke nødvendigvis lønn og miljøkostnader.", "Lavest pris er ikke alltid best; levetid, kvalitet, risiko og eksterne kostnader betyr også noe.", "budsjett", "inntekt", "utgift", "tilbud", "etterspørsel", "handel", "verdikjede", "arbeidsvilkår", "forbruk", "globalisering")
            ],
            ["KRLE"] =
            [
                T("Høytider og tradisjoner", "Høytider uttrykker tro, historie, identitet og fellesskap på varierte måter.", "Samme høytid markeres ulikt mellom land, familier og retninger. Faglig sammenligning skiller opprinnelse, fortelling, symbol og praksis.", "Id al-fitr avslutter ramadan, men mat, klær og familieskikker varierer. «Noen muslimer» er mer presist enn «alle muslimer».", "Ikke alle som tilhører en religion feirer på samme måte.", "høytid", "tradisjon", "ritual", "symbol", "fortelling", "fellesskap", "mangfold", "markering", "hellig", "livssyn"),
                T("Etikk og vennskap", "Etikk undersøker hvordan handlinger påvirker mennesker, relasjoner og rettferdighet.", "I et dilemma kolliderer verdier eller hensyn. Konsekvenser, plikter, omsorg og rettigheter kan gi ulike begrunnelser.", "Hvis en venn ber deg skjule mobbing, kolliderer lojalitet med trygghet. Å hente hjelp kan bryte løftet, men beskytter den utsatte.", "Et godt valg gjør ikke alltid alle fornøyde; etiske valg kan være nødvendige selv når noen blir skuffet.", "etikk", "moral", "dilemma", "verdi", "ansvar", "empati", "rettferdighet", "omsorg", "konsekvens", "integritet"),
                T("Religioner og livssyn", "Religioner og livssyn gir fortellinger, praksiser og tanker om virkelighet og mening.", "Innenfra-perspektiv viser tilhengeres forståelse; utenfra-perspektiv beskriver og analyserer. Begge krever presise kilder og respekt for mangfold.", "Bønn finnes i flere religioner, men formål, ord og praksis varierer. Lik praksisnavn betyr ikke identisk mening.", "Religioner er ikke «egentlig like»; de kan dele trekk og samtidig være grunnleggende forskjellige.", "religion", "livssyn", "gudsbilde", "hellig tekst", "ritual", "tilhenger", "innenfraperspektiv", "utenfraperspektiv", "sekulær", "mangfold"),
                T("Filosofi og store spørsmål", "Filosofi klargjør begreper og prøver argumenter om kunnskap og gode liv.", "Et argument har premisser som skal støtte en konklusjon. Moteksempler tester om en påstand gjelder så bredt som hevdet.", "«Det som er lovlig, er alltid rett» utfordres av historiske diskriminerende lover. Moteksemplet skiller lovlighet fra moral.", "En personlig mening kan fortsatt vurderes; begrunnelser kan være relevante eller selvmotsigende.", "filosofi", "påstand", "premiss", "konklusjon", "argument", "moteksempel", "kunnskap", "sannhet", "frihet", "mening"),
                T("Menneskerettigheter", "Menneskerettighetene beskytter alle menneskers verdighet, frihet, likhet og deltakelse.", "Rettighetene er universelle. Staten skal respektere, beskytte og oppfylle dem. Enkelte begrensninger må ha lovgrunnlag og være nødvendige og forholdsmessige.", "Ytringsfrihet beskytter upopulære meninger, men gir ikke ubegrenset rett til trusler.", "Menneskerettigheter gjelder alle mennesker, ikke bare statsborgere i demokratier.", "menneskeverd", "universell", "diskriminering", "ytringsfrihet", "religionsfrihet", "barnekonvensjonen", "rettighet", "plikt", "rettsvern", "forholdsmessighet")
            ],
            ["Kunst og håndverk"] =
            [
                T("Farge, form og tegning", "Visuelle virkemidler styrer oppmerksomhet, romfølelse, stemning og betydning.", "Farger skaper harmoni eller kontrast. Linje, flate, lys, skygge og perspektiv gjør bilder tydelige eller romlige.", "En varm oransje figur mot kjølig blå bakgrunn får sterk komplementærkontrast og kan virke nærmere.", "Realistisk tegning er ikke alltid bedre; idé, uttrykk og bevisste valg teller også.", "linje", "form", "flate", "primærfarge", "sekundærfarge", "kontrast", "komposisjon", "perspektiv", "lys", "skygge"),
                T("Materialer og teknikker", "Materialegenskaper avgjør hvilke verktøy, sammenføyninger og overflater som passer.", "Tre har fiberretning, tekstil kan rakne, og leire endrer seg ved tørking. Trygg teknikk krever riktig grep, verneutstyr og arbeidsplass.", "En skrue tåler demontering bedre enn lim i en reparerbar trekonstruksjon, og forboring hindrer sprekker.", "Samme verktøy og sammenføyning passer ikke alle materialer.", "materialegenskap", "fiberretning", "sammenføyning", "overflate", "verktøy", "verneutstyr", "presisjon", "tekstil", "tre", "leire"),
                T("Designprosess", "Design utvikles gjennom behov, ideer, prototyper, testing og forbedring.", "Kriterier gjør behov målbare. En prototype skal raskt avsløre problemer før tid og materiale brukes på sluttproduktet.", "En papmodell av et mobilstativ kan avsløre feil vinkel og ustødig fot før modellen bygges i tre.", "Første idé bør ikke bygges ferdig straks; alternativer og testing gir bedre løsninger.", "behov", "målgruppe", "kriterium", "idéutvikling", "skisse", "prototype", "brukertest", "tilbakemelding", "iterasjon", "funksjon"),
                T("Kunst, arkitektur og kultur", "Kunst og arkitektur formes av og kommenterer tid, sted, makt og identitet.", "Form, materiale, symbol og funksjon analyseres sammen med historisk kontekst. Tolkninger må begrunnes i verket, ikke bare smak.", "Et monument viser hvem samfunnet ville minnes da det ble reist; dagens debatt kan vise endrede verdier.", "Kunst har ikke alltid én fasit; flere tolkninger kan være godt begrunnet.", "kunstverk", "arkitektur", "kontekst", "symbol", "estetikk", "funksjon", "identitet", "kulturarv", "makt", "tolkning"),
                T("Bærekraftig design", "Bærekraftig design reduserer ressursbruk og skade gjennom produktets livsløp.", "Materialuttak, produksjon, transport, bruk, reparasjon og avhending må vurderes samlet. Lang levetid og utskiftbare deler er viktig.", "En stol med skrudde standarddeler kan repareres og sorteres lettere enn en limt stol av blandede materialer.", "Et produkt er ikke bærekraftig bare fordi materialet kalles naturlig.", "livsløp", "ressurs", "holdbarhet", "reparasjon", "gjenbruk", "materialgjenvinning", "demontering", "forbruk", "miljømerking", "sirkulær design")
            ],
            ["Musikk"] =
            [
                T("Rytme og puls", "Puls er jevne grunnslag, mens rytme organiserer lyd og pauser over pulsen.", "Taktart grupperer pulsslag, tempo angir hastighet, og synkoper legger trykk på uventede steder. Samme rytme kan spilles i ulikt tempo.", "I 4/4-takt teller vi fire pulsslag. Mønsteret «lang–kort–kort» er rytmen, ikke selve pulsen.", "Puls og rytme er ikke det samme; pulsen er jevn, rytmen kan variere.", "puls", "rytme", "takt", "taktart", "tempo", "pause", "noterverdi", "synkope", "ostinat", "metronom"),
                T("Sang og samspill", "Samspill krever felles puls, lytting, rolleforståelse og tilpasning.", "Intonasjon gjelder tonehøyde, artikulasjon tydelighet og dynamikk styrke. Delene skal støtte helheten.", "Hvis melodien forsvinner bak trommene, kan trommeslageren redusere styrken uten å endre tempo.", "Godt samspill betyr ikke at alle spiller like mye og like sterkt.", "intonasjon", "artikulasjon", "dynamikk", "ensemble", "balanse", "innsats", "stemme", "akkompagnement", "gehør", "samspill"),
                T("Melodi og harmoni", "Melodi er toner i rekkefølge; harmoni oppstår når toner klinger samtidig.", "Skala gir toneforråd, intervall er avstand mellom toner, og akkorder bygges av flere toner. Frasering deler melodien i musikalske setninger.", "En melodi kan ende på grunntonen for å virke avsluttet, mens dominantakkorden skaper forventning.", "En melodi er mer enn toner opp og ned; rytme, frasering og kontrast er avgjørende.", "melodi", "harmoni", "skala", "intervall", "akkord", "grunntone", "frase", "motiv", "toneart", "kadens"),
                T("Komposisjon", "Komposisjon organiserer ideer gjennom gjentakelse, variasjon og kontrast.", "Et motiv er en kort gjenkjennelig idé. Form, som ABA, gjør tiden forståelig for lytteren. Begrensninger kan stimulere kreativitet.", "Et firetoners motiv kan gjentas lysere, spilles baklengs og få ny rytme før originalen vender tilbake.", "Komposisjon trenger ikke en ny idé hvert sekund; bearbeidet gjentakelse skaper gjenkjennelighet.", "motiv", "tema", "variasjon", "kontrast", "form", "ABA", "improvisasjon", "arrangement", "repetisjon", "komposisjon"),
                T("Musikk, kultur og teknologi", "Musikk skapes, spres og får betydning gjennom kultur, historie og teknologi.", "Innspilling og strømming påvirker lyd, arbeidsmåter og økonomi. Sjangre låner og blander trekk på tvers av steder.", "Sampling gjør eldre lyd til materiale i et nytt verk, men reiser kreative og opphavsrettslige spørsmål.", "Teknologi gjør ikke bare musikk enklere; den skaper nye ferdigheter, valg og etiske spørsmål.", "sjanger", "innspilling", "sampling", "strømming", "opphavsrett", "lyddesign", "miks", "kulturutveksling", "identitet", "musikkindustri")
            ],
            ["Kroppsøving"] =
            [
                T("Lek og bevegelse", "Variert lek utvikler bevegelseskompetanse, kreativitet og trygg deltakelse.", "Balanse, koordinasjon, kraft, retning og rytme kombineres ulikt. Regler kan endres for passende utfordring og inkludering.", "I sisten kan flere frisoner og parvis frigjøring gjøre at færre blir stående utenfor.", "Den raskeste løsningen er ikke alltid best; kontroll, samarbeid og sikkerhet kan være viktigere.", "balanse", "koordinasjon", "reaksjon", "retning", "rytme", "romorientering", "bevegelsesglede", "regeltilpasning", "inkludering", "mestring"),
                T("Fair play og samarbeid", "Fair play betyr trygg, rettferdig og inkluderende deltakelse.", "Regler setter rammen, men samarbeid krever kommunikasjon, rollefordeling og respekt. Å gjøre medspillere gode kan øves.", "En spiller som innrømmer at ballen var ute på eget lag, prioriterer rettferdighet foran kortsiktig fordel.", "Fair play handler om mer enn å ikke jukse; språk, omsorg og inkludering teller.", "fair play", "regel", "respekt", "inkludering", "rolle", "kommunikasjon", "lagarbeid", "konfliktløsning", "motstander", "ansvar"),
                T("Friluftsliv og svømming", "Trygg ferdsel ute og i vann bygger på ferdigheter, planlegging og risikovurdering.", "Vær, temperatur, strøm, dybde, terreng og gruppens ferdigheter påvirker risiko. Sporløs ferdsel beskytter naturen.", "En turgruppe som snur når tåke og vind øker, viser god risikostyring – ikke mislykket måloppnåelse.", "Svømmedyktighet alene gjør ikke vannaktivitet trygg; kulde, strøm og tilsyn teller også.", "turplan", "bekledning", "kart", "kompass", "sporløs ferdsel", "svømmedyktighet", "flyte", "strøm", "kameratredning", "risikovurdering"),
                T("Trening og helse", "Trening gir tilpasning når belastning, restitusjon og variasjon balanseres.", "Utholdenhet, styrke, bevegelighet og koordinasjon trenes ulikt. Puls og opplevd anstrengelse kan styre intensitet.", "Fire arbeidsperioder med rolige pauser kan belaste utholdenheten mer målrettet enn samme fart hele økten.", "Smerte er ikke alltid tegn på effektiv trening; skarp eller økende smerte kan varsle skade.", "utholdenhet", "styrke", "bevegelighet", "intensitet", "puls", "belastning", "restitusjon", "progresjon", "oppvarming", "treningsprinsipp"),
                T("Livredning og førstehjelp", "Førstehjelp prioriterer egen sikkerhet, varsling og tidskritiske tiltak.", "En bevisstløs person som ikke puster normalt trenger 113 og HLR. Ved vannredning brukes helst rekkevidde, kasteline eller flytemiddel.", "Sikre stedet, sjekk respons og pust, få noen til å ringe 113, og start 30 brystkompresjoner og 2 innblåsninger.", "Man skal ikke alltid løpe rett bort; en skadet hjelper kan gjøre situasjonen verre.", "egensikkerhet", "bevissthet", "luftvei", "normal pust", "113", "HLR", "hjertestarter", "sideleie", "blødning", "kameratredning")
            ],
            ["Mat og helse"] =
            [
                T("Matglede og kjøkkenhygiene", "Trygg matlaging kombinerer sanser, samarbeid og kontroll av smittefare.", "Håndvask, rene redskaper, skille mellom rått og ferdig og riktig temperatur bryter smitteveier. Smak formes av grunnsmaker, aroma og konsistens.", "Bruk ikke samme uvaskede brett til rå kylling og salat; bakterier kan overføres til mat som ikke varmebehandles.", "Mat er ikke nødvendigvis trygg fordi den lukter normalt.", "håndhygiene", "kryssmitte", "varmebehandling", "kjølekjede", "holdbarhet", "grunnsmak", "aroma", "konsistens", "ren sone", "mattrygghet"),
                T("Kosthold og næringsstoffer", "Et variert kosthold gir energi, byggestoffer og regulerende næringsstoffer over tid.", "Karbohydrat og fett gir energi, protein vedlikeholder vev, og vitaminer, mineraler, vann og fiber støtter kroppens prosesser.", "Havregrøt med melk, bær og nøtter kombinerer karbohydrat, protein, fett, fiber og mikronæringsstoffer.", "Ett næringsstoff eller tilskudd kan ikke erstatte et variert kosthold.", "karbohydrat", "protein", "fett", "vitamin", "mineral", "fiber", "energi", "varedeklarasjon", "porsjon", "kostråd"),
                T("Oppskrifter og måling", "En oppskrift er en arbeidsplan der mengde, rekkefølge, tid og temperatur påvirker resultatet.", "Volum og masse er ulike størrelser. Oppskrifter skaleres med en faktor, men temperatur og steketid følger ikke alltid samme forhold.", "Fra 4 til 6 porsjoner er faktoren 6/4 = 1,5, så 300 g mel blir 450 g.", "Én desiliter veier ikke alltid 100 gram; råvarer har ulik tetthet.", "oppskrift", "råvare", "masse", "volum", "desiliter", "gram", "skalering", "arbeidsrekkefølge", "temperatur", "steketid"),
                T("Matkultur", "Matkultur oppstår mellom råvarer, geografi, historie, tro, teknologi og identitet.", "Retter endres når mennesker og varer flytter. Det finnes variasjon innad i alle matkulturer.", "Poteten ble vanlig i Norge fra 1700-tallet og endret kostholdet; det vi kaller tradisjon, har også en historie med endring.", "Tradisjonell mat står ikke stille; migrasjon, teknologi og smak endrer tradisjoner.", "matkultur", "tradisjon", "identitet", "råvaretilgang", "måltid", "religion", "migrasjon", "kulturutveksling", "lokalmat", "kilde"),
                T("Bærekraftige matvalg", "Bærekraftige matvalg vurderer klima, natur, dyrevelferd, helse og økonomi samlet.", "Matsvinn sløser alle ressursene bak maten. Sesong, produksjon, transport og emballasje betyr noe, men totalbildet varierer.", "Å lage middag av grønnsaker som snart går ut på dato, reduserer svinn; riktig oppbevaring forlenger holdbarheten.", "Kortreist mat har ikke alltid lavest klimaavtrykk; produksjonsmåte kan bety mer enn transport.", "matsvinn", "sesong", "holdbarhet", "best før", "siste forbruksdag", "plantebasert", "dyrevelferd", "klimaavtrykk", "emballasje", "ressursbruk")
            ],
            ["Fremmedspråk"] =
            [
                T("Hilsener og presentasjon", "Faste fraser og aktiv lytting gjør det mulig å etablere kontakt tidlig.", "Hilsen, navn, bosted og interesser uttrykkes med språkets egne høflighets- og ordstillingsmønstre. Situasjonen avgjør formell eller uformell form.", "En presentasjon kan bygges som hilsen + navn + bosted + interesse + spørsmål, slik at samtalen fortsetter.", "Ord-for-ord-oversettelse gir ikke alltid en naturlig setning.", "hilsen", "presentasjon", "høflighet", "formell", "uformell", "spørreord", "svarfrase", "tiltale", "samtalestart", "avskjed"),
                T("Ordforråd og uttale", "Ord læres gjennom mening, gjenhenting, uttale og gjentatt bruk.", "Lydsystemet kan skille betydning annerledes enn norsk. Ordnettverk, orddeler og kontekst hjelper forståelse og hukommelse.", "Lær ordet for «reise» sammen med transportmidler, en setning og et lydopptak – ikke bare i en isolert liste.", "Aksent betyr ikke at språket er feil; forståelighet er viktigere enn å kopiere én gruppe.", "uttale", "språklyd", "trykk", "intonasjon", "ordfamilie", "kontekst", "ordnettverk", "gjenhenting", "strategi", "forståelighet"),
                T("Grunnleggende grammatikk", "Grammatiske mønstre viser hvem som gjør hva og når.", "Subjekt, verb og andre ledd kan stå annerledes enn på norsk. Bøying kan markere person, kjønn, tall eller tid.", "Når verbet endrer form med personen, må både subjekt og verb kontrolleres; ordbokformen alene er ikke nok.", "Norsk ordstilling kan ikke alltid beholdes selv om hvert ord oversettes riktig.", "subjekt", "verb", "ordstilling", "bøying", "person", "kjønn", "tall", "tid", "samsvar", "setningsmønster"),
                T("Kultur og samfunn", "Språk gir tilgang til perspektiver, men kultur må undersøkes uten generalisering.", "Medier, skole, familieformer og ungdomskultur varierer mellom og innen språkområder. Kilder må plasseres i tid og sted.", "En video fra én skole viser en virkelig erfaring, men kan ikke beskrive skolehverdagen til alle i landet.", "Et land har ikke én kultur som alle innbyggere følger.", "språkområde", "kultur", "samfunn", "identitet", "perspektiv", "mangfold", "hverdagsliv", "kilde", "stereotypi", "kulturuttrykk"),
                T("Samtale og tekst", "Kommunikasjon lykkes når mening planlegges og form tilpasses mottakeren.", "Omskriving, eksempel, kroppsspråk og oppklaringsspørsmål holder samtalen i gang. Tekster trenger sammenhengsord og revisjon.", "Mangler ordet for paraply, kan du si «tingen jeg bruker når det regner» og få hjelp uten å gi opp.", "En samtale trenger ikke være grammatisk feilfri for å være vellykket.", "mottaker", "formål", "sammenhengsord", "omskriving", "oppklaringsspørsmål", "respons", "flyt", "teksttype", "revisjon", "kommunikasjonsstrategi")
            ],
            ["Utdanningsvalg"] =
            [
                T("Interesser og styrker", "Utdanningsvalg bygger på interesser, verdier, ferdigheter og utforsking – ikke én test.", "Interesser utvikles og ferdigheter trenes. Erfaring og tilbakemelding gir bedre selvinnsikt enn faste merkelapper.", "En som liker skolearrangementer kan utforske prosjektledelse, service, økonomi og kreative yrker før valget snevres inn.", "Man trenger ikke finne ett perfekt yrke for alltid; karriere utvikles gjennom mange valg.", "interesse", "styrke", "ferdighet", "verdi", "motivasjon", "selvinnsikt", "erfaring", "tilbakemelding", "utforsking", "karriere"),
                T("Videregående opplæring", "Videregående har studieforberedende og yrkesfaglige løp med ulike veier videre.", "Studiekompetanse kvalifiserer til høyere utdanning, mens yrkeskompetanse ofte kombinerer skole og læretid. Påbygg og omvalg gir flere ruter.", "En yrkesfagelev kan gå ut i lære og ta fagbrev, og senere velge fagskole, y-vei eller påbygg.", "Valg av Vg1 låser ikke hele arbeidslivet, selv om det har reelle konsekvenser.", "utdanningsprogram", "studiekompetanse", "yrkeskompetanse", "læretid", "fagbrev", "påbygg", "fagskole", "omvalg", "inntak", "vitnemål"),
                T("Arbeidsliv og yrker", "Arbeidslivet trenger faglig, sosial og omstillingsdyktig kompetanse.", "En yrkestittel viser ikke hele hverdagen. Oppgaver, arbeidstid, ansvar, teknologi og arbeidsmiljø må undersøkes konkret.", "En elektriker bruker håndlag, matematikk, dokumentasjon, kundekommunikasjon og sikkerhetsrutiner.", "Skolefag og arbeidsliv er ikke adskilt; lesing, regning og samarbeid brukes praktisk.", "arbeidsoppgave", "kompetanse", "arbeidsmiljø", "arbeidstid", "teknologi", "samarbeid", "fagorganisering", "ansettelse", "omstilling", "livslang læring"),
                T("Valg og konsekvenser", "Et informert valg sammenligner alternativer, konsekvenser og usikkerhet mot egne prioriteringer.", "Valg påvirkes av venner og forventninger. En beslutningsmatrise synliggjør avveininger, men velger ikke for deg.", "To skoler kan sammenlignes etter faginnhold, miljø, reisetid og veier videre, med ulik vekt på kriteriene.", "Flest fordeler på en liste gir ikke automatisk riktig valg; noen kriterier veier mer.", "alternativ", "kriterium", "konsekvens", "prioritering", "beslutning", "usikkerhet", "påvirkning", "informasjon", "rådgivning", "handlingsplan")
            ],
            ["Valgfag"] =
            [
                T("Idé og prosjekt", "Et prosjekt gjør en idé gjennomførbar med mål, leveranser, roller, tid og risiko.", "Et mål må kunne vurderes. Milepæler deler arbeidet, og risikoanalyse gjør gruppen klar for hindringer.", "«Lag en fem minutters podkast for nye elever innen fredag» er tydeligere enn «lag noe om skolen».", "En startplan kan endres når ny kunnskap gir gode grunner.", "idé", "behov", "mål", "leveranse", "rolle", "milepæl", "framdrift", "ressurs", "risiko", "evaluering"),
                T("Praktisk skapende arbeid", "Skapende arbeid utvikles gjennom utprøving, presisjon og forbedrede versjoner.", "Materiale, verktøy og teknikk må passe funksjonen. Dokumenterte prøver gjør løsninger sammenlignbare.", "En prototype som knekker i skjøten viser hvor konstruksjonen må forsterkes og gir nyttig kunnskap.", "Feil bør ikke skjules; forsøk viser utvikling og gir bedre valg.", "materiale", "verktøy", "teknikk", "prototype", "funksjon", "kvalitet", "presisjon", "utprøving", "dokumentasjon", "forbedring"),
                T("Samarbeid og formidling", "Samarbeid lykkes når ansvar, målgruppe, budskap og respons er tydelig.", "Aktiv lytting og konkrete avtaler styrker gruppen. Medium og språk velges etter hva mottakeren trenger.", "En video for yngre elever trenger korte trinn og bilder; en rapport til ledelsen trenger begrunnelser og dokumentasjon.", "Godt samarbeid krever ikke at alle gjør samme oppgave; rettferdig bidrag kan ha ulike roller.", "samarbeid", "rolle", "ansvar", "aktiv lytting", "målgruppe", "budskap", "medium", "respons", "konflikt", "formidling"),
                T("Entreprenørskap", "Entreprenørskap utvikler løsninger som skaper verdi for en bruker eller et samfunnsbehov.", "Behov må undersøkes, løsningen testes og ressursbruk og pris vurderes. Verdi kan være sosial, kulturell eller økonomisk.", "En byttetjeneste for sportsutstyr kan spare penger og avfall; en liten pilot tester om noen faktisk vil delta.", "En god idé blir ikke automatisk brukt; behov, gjennomføring og tillit må fungere.", "behov", "bruker", "verdiforslag", "idé", "prototype", "pilot", "ressurs", "kostnad", "inntekt", "samfunnsverdi")
            ],
            ["Arbeidslivsfag"] =
            [
                T("Arbeidsoppdrag og kvalitet", "Et arbeidsoppdrag oversettes fra behov til krav, plan, utførelse og kontroll.", "Kvalitetskriterier gjelder mål, funksjon, toleranse, finish og dokumentasjon. Kontroll underveis forebygger dyre feil.", "Ved bygging av en hylle kontrolleres mål og vinkel før delene festes; sluttkontroll kan være for sent.", "Kvalitet betyr mer enn pent utseende; funksjon, sikkerhet og mål teller.", "bestilling", "krav", "arbeidstegning", "toleranse", "arbeidsrekkefølge", "egenkontroll", "avvik", "finish", "dokumentasjon", "kvalitet"),
                T("Helse, miljø og sikkerhet", "HMS forebygger skade ved å finne fare, vurdere risiko og velge tiltak.", "Risiko kombinerer sannsynlighet og konsekvens. Fjerning av faren er bedre enn verneutstyr alene.", "Ved trestøv er avsug ved kilden et bedre hovedtiltak enn bare støvmaske.", "Personlig verneutstyr gjør ikke enhver arbeidsmåte trygg.", "fare", "risiko", "sannsynlighet", "konsekvens", "vernetiltak", "verneutstyr", "sikkerhetsdatablad", "avvik", "nestenulykke", "ergonomi"),
                T("Samarbeid på arbeidsplassen", "Profesjonelt samarbeid bygger på avtaler, faglig kommunikasjon, ansvar og respekt.", "Arbeidsplassen trenger rolleavklaring, overlevering og tilbakemelding. Taushetsplikt og personvern begrenser deling.", "Ved vaktskifte beskrives en uferdig oppgave med status, risiko og neste steg – ikke bare «noe står igjen».", "Det er ikke profesjonelt å skjule feil; tidlig varsling begrenser skade.", "rolle", "ansvar", "arbeidskultur", "fagspråk", "overlevering", "tilbakemelding", "taushetsplikt", "personvern", "konflikt", "avviksmelding"),
                T("Råvarer, verktøy og økonomi", "Råvare, verktøy og metode påvirker kvalitet, kostnad, tid, sikkerhet og miljø.", "Materialsvinn, arbeidstid, innkjøpspris og levetid inngår i kostnaden. Riktig verktøy øker presisjon og reduserer feil.", "Et dyrere bor som varer lenge kan koste mindre per oppdrag enn et billig bor som ofte må byttes.", "Lav innkjøpspris gir ikke alltid best økonomi; tid, feil og levetid påvirker totalkostnaden.", "råvare", "materialegenskap", "verktøyvalg", "mengdeberegning", "svinn", "innkjøpspris", "arbeidstid", "kalkyle", "vedlikehold", "totalkostnad")
            ]
        };

    private static readonly IReadOnlyDictionary<string, string> Methods = new Dictionary<string, string>
    {
        ["Norsk"] = "Les for helhet, marker tekstbevis, navngi språklige trekk og begrunn tolkningen med presise eksempler.",
        ["Matematikk"] = "Forstå situasjonen, velg representasjon og regneart, vis mellomregning, og kontroller med overslag eller innsetting.",
        ["Engelsk"] = "Read or listen for context, notice the language pattern, choose evidence, produce a complete response, and revise it for clarity.",
        ["Naturfag"] = "Formuler spørsmål, identifiser system og variabler, bruk observasjoner eller data, forklar årsakskjeden og vurder usikkerhet.",
        ["Samfunnsfag"] = "Avgrens saken, finn aktører og perspektiver, undersøk kilder og sammenhenger, og begrunn en nyansert konklusjon.",
        ["KRLE"] = "Beskriv presist, skill innenfra- og utenfraperspektiv, sammenlign uten å rangere, og begrunn etiske eller filosofiske vurderinger.",
        ["Kunst og håndverk"] = "Avklar uttrykk eller funksjon, skisser alternativer, prøv materiale og teknikk, vurder mot kriterier og forbedre.",
        ["Musikk"] = "Lytt etter ett virkemiddel om gangen, marker puls eller form, prøv langsomt, spill inn, og juster etter det du faktisk hører.",
        ["Kroppsøving"] = "Avklar oppgaven, vurder sikkerhet, prøv kontrollert, observer egen og andres bevegelse, og tilpass teknikk eller regler.",
        ["Mat og helse"] = "Les hele arbeidsplanen, organiser hygienisk, mål presist, følg kritisk rekkefølge og vurder smak, trygghet og ressursbruk.",
        ["Fremmedspråk"] = "Finn formål og mottaker, bruk lærte språkmønstre, lytt etter nøkkelord, reparer misforståelser og prøv uttrykket på nytt.",
        ["Utdanningsvalg"] = "Samle oppdatert informasjon og egne erfaringer, lag kriterier, sammenlign alternativer, be om veiledning og planlegg neste steg.",
        ["Valgfag"] = "Avklar behov og målgruppe, lag kriterier og plan, bygg en enkel versjon, test med brukere og forbedre dokumentert.",
        ["Arbeidslivsfag"] = "Avklar krav, vurder HMS, planlegg rekkefølge og ressurser, utfør med kontrollpunkter, og dokumenter resultat og avvik."
    };

    private static readonly IReadOnlyDictionary<string, string> CurriculumUrls = new Dictionary<string, string>
    {
        ["Norsk"] = "https://www.udir.no/lk20/nor01-06", ["Matematikk"] = "https://www.udir.no/lk20/mat01-05",
        ["Engelsk"] = "https://www.udir.no/lk20/eng01-05", ["Naturfag"] = "https://www.udir.no/lk20/nat01-04",
        ["Samfunnsfag"] = "https://www.udir.no/lk20/saf01-04", ["KRLE"] = "https://www.udir.no/lk20/rle01-03",
        ["Kunst og håndverk"] = "https://www.udir.no/lk20/khv01-02", ["Musikk"] = "https://www.udir.no/lk20/mus01-02",
        ["Kroppsøving"] = "https://www.udir.no/lk20/kro01-05", ["Mat og helse"] = "https://www.udir.no/lk20/mhe01-02",
        ["Fremmedspråk"] = "https://www.udir.no/lk20/fsp01-04", ["Utdanningsvalg"] = "https://www.udir.no/lk20/utv01-03",
        ["Valgfag"] = "https://www.udir.no/laring-og-trivsel/lareplanverket/",
        ["Arbeidslivsfag"] = "https://www.udir.no/lk20/arb01-03"
    };

    public static IReadOnlyList<string> SubjectsForGrade(int grade)
    {
        var subjects = new List<string> { "Norsk", "Matematikk", "Engelsk", "Naturfag", "Samfunnsfag", "KRLE", "Kunst og håndverk", "Musikk", "Kroppsøving", "Mat og helse" };
        if (grade >= 8) subjects.AddRange(["Fremmedspråk", "Utdanningsvalg", "Valgfag", "Arbeidslivsfag"]);
        return subjects;
    }

    public static IEnumerable<Course> CreateCourses()
    {
        for (var grade = 1; grade <= 10; grade++)
        foreach (var subject in SubjectsForGrade(grade))
        {
            var topics = Topics[subject];
            var first = grade >= 8 && subject is "Fremmedspråk" or "Utdanningsvalg" or "Valgfag" or "Arbeidslivsfag"
                ? grade - 8 : grade <= 2 ? 0 : grade <= 4 ? 1 : grade <= 7 ? 2 : 3;
            for (var number = 0; number < 2; number++)
            {
                var topic = topics[(first + number) % topics.Length];
                var difficulty = grade <= 3 ? CourseDifficulty.Lett : grade <= 7 ? CourseDifficulty.Middels : CourseDifficulty.Utfordrende;
                yield return BuildCourse(grade, subject, topic, topics, difficulty, number);
            }
        }
    }

    public static GameVocabulary GetGameVocabulary(Course course)
    {
        var topics = Topics.GetValueOrDefault(course.Subject, Topics["Norsk"]);
        var topic = topics.FirstOrDefault(x => x.Name == course.Title) ?? topics[0];
        var pedagogy = AgeAdaptedPedagogy.Create(course.Grade, course.Subject, topic, Methods[course.Subject]);
        var adaptedTerms = AgeAdaptedPedagogy.GetTerms(course.Grade, course.Subject, topic);
        var distractors = topics.Where(x => x != topic).SelectMany(x => x.Terms).Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(x => !topic.Terms.Contains(x, StringComparer.OrdinalIgnoreCase)).ToArray();
        var pairLabels = course.Grade <= 2
            ? new[] { "Dette lærer vi", "Slik virker det", "Se dette", "Pass på", "Slik kan du prøve" }
            : course.Grade <= 4
                ? new[] { "Hovedidé", "Forklaring", "Eksempel", "Viktig å huske", "Arbeidsmåte" }
                : new[] { "Kjerneidé", "Faglig forklaring", "Gjennomarbeidet eksempel", "Vanlig misforståelse", "Nyttig arbeidsmåte" };
        var pairs = new[]
        {
            new GamePair(pairLabels[0], pedagogy.Topic.Core),
            new GamePair(pairLabels[1], pedagogy.Topic.Explanation),
            new GamePair(pairLabels[2], pedagogy.Topic.Example),
            new GamePair(pairLabels[3], pedagogy.Topic.Reminder),
            new GamePair(pairLabels[4], pedagogy.Method)
        };
        string[] sequence = course.Grade <= 2
            ? new[] { "Se eller lytt.", "Finn det viktigste.", "Prøv med ting, bilde eller bevegelse.", "Tegn eller vis hva som skjedde.", "Fortell og sjekk sammen med noen." }
            : [.. pedagogy.Steps, course.Grade <= 4 ? "Forbedre forklaringen etter kontrollen." : "Vurder en mulig feil eller innvending og presiser konklusjonen."];
        var intro = course.Grade <= 2
            ? $"Lek deg gjennom «{topic.Name}»: finn riktige kort, koble bilde og forklaring, og bygg stegene i riktig rekkefølge."
            : course.Grade <= 4
                ? $"Tre spill lar deg sortere, koble og bygge en modell av «{topic.Name}»."
                : $"Tre ulike oppdrag trener «{topic.Name}»: sorter kort, koble sammen forklaringer og bygg et faglig puslespill i riktig rekkefølge.";
        return new(intro,
            adaptedTerms, distractors, pairs, sequence);
    }

    private static Course BuildCourse(int grade, string subject, AcademicTopic topic, AcademicTopic[] topics,
        CourseDifficulty difficulty, int sortOrder)
    {
        var pedagogy = AgeAdaptedPedagogy.Create(grade, subject, topic, Methods[subject]);
        var course = new Course
        {
            Grade = grade, Subject = subject, Title = topic.Name,
            Summary = pedagogy.Summary,
            Content = BuildContent(grade, subject, topic), CatalogVersion = ContentVersion,
            Difficulty = difficulty, SortOrder = sortOrder, IsPublished = true
        };
        course.Questions = AcademicQuestionCatalog.Build(grade, subject, topic, topics, sortOrder);
        return course;
    }

    private static string BuildContent(int grade, string subject, AcademicTopic topic)
    {
        var pedagogy = AgeAdaptedPedagogy.Create(grade, subject, topic, Methods[subject]);
        var adaptedTerms = AgeAdaptedPedagogy.GetTerms(grade, subject, topic);
        var terms = string.Join("", adaptedTerms.Take(pedagogy.TermCount).Select((x, index) =>
            $"<li>{(grade <= 2 ? $"<span aria-hidden=\"true\">{new[] { "●", "▲", "■", "◆", "★" }[index % 5]}</span>" : string.Empty)}{E(x)}</li>"));
        var steps = string.Join("", pedagogy.Steps.Select(x => $"<li>{E(x)}</li>"));
        var coreLabel = grade <= 2 ? "Husk dette:" : grade <= 4 ? "Hovedidé:" : "Kjerneidé:";
        var reminderLabel = grade <= 2 ? "Husk:" : "Pass på:";
        return $"""
            <div data-content-version="{ContentVersion}" data-grade-band="{(grade <= 2 ? "early" : grade <= 4 ? "primary" : grade <= 7 ? "middle" : "secondary")}">
            <h2>1. Dette skal du lære</h2>
            <p>{E(pedagogy.Goal)}</p>
            <div class="fact-box"><strong>{coreLabel}</strong> {E(pedagogy.Topic.Core)}</div>
            <h2>2. {pedagogy.ExplanationHeading}</h2><p>{E(pedagogy.Topic.Explanation)}</p><p>{E(pedagogy.StudyAdvice)}</p>
            {pedagogy.VisualHtml}
            <h3>{(grade <= 2 ? "Ord vi øver på" : "Nøkkelbegreper")}</h3><ul class="term-list">{terms}</ul>
            <h2>3. {pedagogy.ExampleHeading}</h2><p>{E(pedagogy.Topic.Example)}</p>
            <p><strong>{(grade <= 2 ? "Hva ser vi?" : "Hvorfor eksemplet virker:")}</strong> {(grade <= 2 ? "Eksemplet viser hovedideen med noe du kan se, gjøre eller kjenne igjen." : "Det kobler en konkret situasjon til hovedideen og viser hvilke opplysninger eller begreper som bærer forklaringen.")}</p>
            <h2>4. {pedagogy.MethodHeading}</h2><p>{E(pedagogy.Method)}</p>
            <ol class="learning-steps">{steps}</ol>
            <h2>5. {(grade <= 2 ? "Noe det er lett å blande" : "Vanlig misforståelse")}</h2><div class="misconception-box"><strong>{reminderLabel}</strong> {E(pedagogy.Topic.Reminder)}</div>
            <h2>6. Din utfordring</h2><p>{E(pedagogy.Challenge)}</p><p>{E(pedagogy.ResponsePrompt)}</p>
            <p class="source-note">Faglig retning: <a href="{CurriculumUrls[subject]}" target="_blank" rel="noopener">LK20-læreplanen i {E(subject)}</a>. Innholdet er et læringssupplement og erstatter ikke lærerens vurdering eller skolens lokale plan.</p>
            </div>
            """;
    }

    private static string E(string value) => WebUtility.HtmlEncode(value);
}
