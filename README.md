# LearnPlane

LearnPlane er en norskspråklig læringsplattform for elever fra 1. til 10. klasse. Løsningen er en Blazor Web App med PostgreSQL, innlogging, kurs, quiz, læringsspill, poeng, belønningsbutikk, resultatliste og administrasjon.

## Funksjoner

- Alle elever får tilgang til alle trinn og fag, med filtrering på klasse og fag.
- Nye elever registrerer alder. Normalprogresjonen 6 år = 1. klasse brukes til poengreglene; kurs under elevens nåværende trinn kan gjennomføres fritt, men gir ikke poeng.
- To kurs med fire quizspørsmål per fag og trinn opprettes automatisk første gang appen starter.
- Minst 70 % riktige svar gir bestått og ¾ av poengpotten. 100 % gir den siste fjerdedelen. Quiz og spill kan gjentas ubegrenset, men hver del av potten deles bare ut én gang.
- Hvert kurs har en aldersjustert begrepsjakt i tre nivåer. Nivåene låses opp i rekkefølge og gir 1–20 poeng per nivå, avhengig av vanskelighetsgrad.
- Resultatlisten bruker beste quizresultat per kurs og beste spillresultat per nivå, og viser aldri antall forsøk.
- Elever kan bruke tilgjengelige quizpoeng på administratorstyrte belønninger gjennom en persistent handlekurv.
- Kjøp lagres med historiske navn, bilder og priser, mens resultatlisten fortsatt viser opptjente poeng uavhengig av forbruk.
- Administrator kan se alle gjennomføringer, endre brukeres visningsnavn og passord, og redigere kurs, læringstekst, spørsmål og svaralternativer.
- Administrator kan opprette og redigere belønninger med rasterbilde, beskrivelse, aktiv status og pris i poeng, samt se de siste kjøpene.
- Brukergrensesnitt og læringsinnhold er på norsk.

## Start med Docker Compose

Du trenger Docker med Compose-plugin.

```bash
cp .env.example .env
# Bytt POSTGRES_PASSWORD i .env før appen eksponeres på nettverket.
docker compose up --build -d
```

Åpne `http://<serverens-ip>:8080`. Porten kan endres med `LEARNPLANE_PORT` i `.env`.

Løsningen bruker nøyaktig to hovedcontainere:

- `learnplane-webapp`: Blazor/.NET 10
- `learnplane-database`: PostgreSQL 17 med persistent Docker-volum

Databaseskjema, roller, kurs, spill, en eksempelbelønning og administrator opprettes automatisk. Alders-, spill- og belønningsskjema oppgraderes også automatisk for eksisterende installasjoner. Stopp med `docker compose down`. Database og innloggingsnøkler beholdes i Docker-volumer; `docker compose down -v` sletter også disse dataene permanent.

## Innlogging og sikkerhet

Førstegangs administrator er:

- Brukernavn: `admin`
- Passord: `3d9XehYf`

Passordet kan overstyres med `ADMIN_INITIAL_PASSWORD` før første oppstart og bør endres umiddelbart via **Administrasjon → Brukere**. Vanlige brukere oppretter konto fra samme innloggingsportal. Passord lagres som Identity-hasher, aldri i klartekst.

Denne første lokale versjonen bruker HTTP. Ved tilgang utenfor et betrodd hjemmenett bør den plasseres bak en HTTPS-reverse proxy, og serverens brannmur bør begrense tilgangen.

## Læreplangrunnlag

Kurskatalogen følger fagene og prinsippene i Kunnskapsløftet 2020 (LK20). Halden kommune publiserer fagplanmateriale for enkelte skoler/trinn, men ikke én samlet offentlig lokal fagplan for alle fag på 1.–10. trinn. Derfor er nasjonale læreplaner brukt som autoritativ grunnstruktur, supplert med tilgjengelig materiale fra Halden:

- [Planer for barn og skoler – Halden kommune](https://www.halden.kommune.no/tjenester/undervisning-og-oppvekst/planer-for-barn-og-skoler/)
- [Eksempel: fagplan i norsk, 8. trinn – Halden kommune](https://www.halden.kommune.no/_f/idabdd6a7-a60d-42b4-aeae-2ffbee5237bf/fagplan-norsk-23-24.pdf)
- [Fag- og timefordeling – Utdanningsdirektoratet](https://www.udir.no/laring-og-trivsel/lareplanverket/fag-og-timefordeling/)
- [Helheten i LK20 – Utdanningsdirektoratet](https://www.udir.no/laring-og-trivsel/lareplanverket/stotte/helhet-lareplanverket/)

Det innebygde innholdet er et funksjonelt startbibliotek og et læringssupplement. Administratorverktøyet er laget for videre faglig kvalitetssikring og utvidelse sammen med lærere.

## Lokal utvikling og test

```bash
dotnet restore LearnPlane.slnx
dotnet test LearnPlane.slnx
dotnet run --project src/LearnPlane.Web
```

Standard lokal tilkobling forventer PostgreSQL på `localhost:5432`; den kan overstyres med `ConnectionStrings__DefaultConnection`.
