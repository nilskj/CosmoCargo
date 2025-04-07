🚀 Kodtest: CosmoCargo™ – Intergalaktisk Fraktcentral 

Bakgrund 

CosmoCargo™ är den ledande aktören inom rymdlogistik. Med leveranser till över 9000 rymdstationer i galaxen finns ett sofistikerat system på plats för att hantera bokningar. 

Du har blivit anlitad för att bygga en ny feature i CosmoCargo-plattformen, med fokus på användarvänlighet, tydlig struktur och god kodkvalitet. Systemet används av kunder, piloter och administratörer. 

 

🛠️ Funktionella krav 

🎨 Frontend 

Inloggning/registrering med olika roller: Kund, Pilot, Admin 

Kund vy: 

Som kund kan man boka/skapa en frakt genom att fylla i ett formulär (avsändare, mottagare, vikt, kategori, prioritet) 

Man kan se sina pågående och tidigare leveranser 

Spåra ett paket i realtid (simulerad status) 

Pilot vy: 

Som pilot kan man se sina tilldelade frakter. Då visas status, datum och destination. 

Som pilot kan man markera en frakt som "påbörjad", "pågående", "levererad" 
 

Admin dashboard: 

Som admin ser man alla frakter, de ska kunna filtreras på olika parametrar. 

En admin ska kunna tilldela en pilot till en frakt. 

Admin ska även kunna ge den status “lost in black hole”. 

 

 

📝 Uppgift Tullformulär: “Galactic Cargo Declaration” 

Universum står inför ett handelskrig och inför intergalaktiska tullar. CosmoCargo behöver ta fram ett system för tullhantering. 

Kund skickar in tullformulär i samband med att man bokar frakt. 

Admin ska kunna granska tullformulären.  

Piloten ska kunna se om frakten är högriskklassad 

 

🧩 Formulärfält 

Fält 

Typ 

Beskrivning 

shipmentId 

UUID 

Referens till det aktuella fraktobjektet 

containsLifeforms 

Boolean 

Anger om frakten innehåller levande varelser 

lifeformType 

Text 

(Om ja) Beskrivning av art, intelligensnivå, och riskklass 

isPlasmaActive 

Boolean 

Indikerar om varan innehåller plasma-aktiva material 

plasmaStabilityLevel 

Number (1–10) 

(Om ja) Skala över stabiliteten i materialet 

originPlanetLawsConfirmed 

Checkbox 

Användaren intygar att exporten följer ursprungsplanetens lagar 

quarantineRequired 

Boolean 

Behövs särskild karantänzon vid ankomst? 

customsNotes 

Textarea 

Frivillig fritext för tullinspektörer 

submittedBy 

Auto 

Namn/ID på inloggad användare (kopplas automatiskt) 

 

🛡️ Valideringsregler (Frontend & Backend) 

containsLifeforms === true ⇒ lifeformType måste fyllas i (obligatoriskt) 

isPlasmaActive === true ⇒ plasmaStabilityLevel måste anges (1–10) 

Om plasmaStabilityLevel < 4 ⇒ frakten måste markeras som karantänsklassad 

originPlanetLawsConfirmed måste vara ikryssad, annars får formuläret inte skickas 

Textfält har begränsningar i antal tecken för att motverka injection eller dataexplosion 

 

💥 Automatiserad Riskbedömning 

Beskrivning: 
 Bygg en funktion (frontend eller backend) som automatiskt klassificerar fraktrisknivå baserat på formulärets innehåll och returnerar en flaggning som visas för admin. 

Riskklassificeringsexempel: 

Villkor 

Risknivå 

Plasmaaktiv + stabilitet < 5 + innehåller livsform 

Kritisk 

Livsform + okänd art (saknar typ eller “?” i beskrivningen) 

Hög 

Plasmaaktiv med stabilitet mellan 5–7 

Medel 

Inga specialegenskaper 

Låg 

 

 

🔧 Backend 

Autentisering (JWT eller liknande) 

REST API eller GraphQL med tydliga endpoints för: 

Fraktbokning 

Fraktstatus 

Användarroller och autentisering 

Tullformulär 

Fraktdata innehåller: 

Unik ID, avsändarplanet, destinationsplanet, vikt, typ (livsmedel, teknik, varelser), risknivå, status 

Simulera fraktstatus över tid (t.ex. cron-jobb eller endpoint som uppdaterar status efter x minuter) 
 

🗄️ Databasförslag 

PostgreSQL eller MongoDB 

Tre huvudsakliga modeller: User, Shipment, TollForm 

 

⚙️ Infrastruktur / DevOps 

Dockerisera hela applikationen (frontend, backend, databas) 

docker-compose.yml för enkel uppstart 

CI/CD (GitHub Actions) som: 

Lintar kod 

Kör tester (minst enhetstester på backend) 

Bygger och deployar projektet till molntjänst (Railway, Vercel, etc.) 

README med instruktioner för: 

Setup lokalt 

Användning av API 

Rollbaserad åtkomst 

 

⭐ Bonusutmaningar 

Realtidsuppdatering med WebSockets (för fraktstatus) 

Visuell rymdkarta med destinationer 

Möjlighet att “försäkra” en leverans (extra avgift & tracking) 

Lägg till statistik / diagram till Admin-dashboard (t.ex. antal transporter per planet, mest använda rutter) 

Chat-bot (kundsupport, kan exempelvis fråga “Vart är mitt paket?”) 

 

🧪 Bedömningskriterier 

Funktionalitet & kravuppfyllnad 

Responsivitet 

Arkitektur & kodkvalitet 

UI/UX och användarvänlighet 

DevOps & struktur 

Dokumentation och onboarding 