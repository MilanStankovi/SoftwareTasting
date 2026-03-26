# Frontend Playwright tests (End-to-End + API)

Ovaj folder sadrži nezavisnu komponentu za Playwright testove napisanih u .NET (NUnit + Microsoft.Playwright).

Preduslovi:
- .NET 10 SDK
- Docker (ako želite da pokrenete backend i bazu iz repo-a)
- Playwright browsere (možete ih instalirati pre pokretanja testova)

Brzi start:

1. (opciono) Pokrenite backend i mongodb iz repo-a iz foldera `test`:

```powershell
cd test
docker compose up -d
```

2. (opciono) Servirajte front-end (repo sadrži `frontend` folder). Možete koristiti bilo koji static server, npr. `http-server` iz npm-a ili VS Code Live Server.

3. Restore i pokretanje testova:

```powershell
cd test/frontend-playwright
dotnet restore
dotnet test
```

4. Ako Playwright browsere nisu instalirane, instalirajte ih jednom (ako imate `playwright` CLI):

```powershell
dotnet tool install --global Microsoft.Playwright.CLI
playwright install
```

Promenljive okruženja:
- `FRONTEND_URL` — URL front-enda (podrazumevano `http://localhost:8080`)
- `API_URL` — URL backend API-ja (podrazumevano `http://localhost:5000`)

Napomena:
- Projekat je namenjen da se izvršava nezavisno od glavnog rešenja; testovi koriste Playwright API za E2E i HTTP/REST pozive.
