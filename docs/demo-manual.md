# Demo manual - La Olla (5 minutes)

Fixture: database created with `sql/01_schema_seed.sql` (batch day+1, quota 10,
admin `12345678`, student `87654321`). Connection string set in
`src/LaOlla.Vista/App.config`. Build on the uni PC, run `LaOlla.Vista`.

## Script

| Min | What to show |
| --- | ------------ |
| 0-1 | Problem in one sentence: the paper notebook overpromises lunches. Rule R1: the day starts capped at 150 and the system never promises beyond quota. Code: `ViandaController.CargarVianda` + `ReservaController.Reservar` (transaction in `ReservaRepository.ReservarAtomico`). |
| 1-3 | Flow: login as admin `12345678` -> counter view shows batch day+1 quota 10. Login as `87654321` -> reserve. Back in admin, refresh: available 9. Fill to 10, attempt one more -> message `Cupo agotado`. |
| 3-4 | Code: `ReservaController.Reservar` (R1/R2/R3 checks) calls `ReservaRepository.ReservarAtomico` (re-checks inside the transaction); the form only shows `Resultado.Mensaje`. Quota cap 150 rejected in `ViandaController.CargarVianda`. |
| 4-5 | Spoken assumption: single batch per day (S1); if the canteen ever serves two menus a day the `Vianda` key must change. |

## Manual checklist (no automated tests by scope decision)

- [ ] `dotnet build src/LaOlla.Modelo src/LaOlla.Controlador` passes on Linux.
- [ ] Vista builds and runs on the uni PC; login `12345678` opens the counter view.
- [ ] Batch loads with quota 10; quota 151 is rejected.
- [ ] Student reserves; duplicate (same Fecha+DNI) is rejected.
- [ ] Filling to quota + 1 shows `Cupo agotado`.
- [ ] Cancel releases quota (row becomes `Cancelada`, available +1).
- [ ] Deliver marks `Entregado`; after 14:00 pending become `No retirado`.
- [ ] Data persists after reopening the app.
