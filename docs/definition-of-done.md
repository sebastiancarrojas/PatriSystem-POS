# Definition of Done

Cuándo se considera terminado un ticket y un sprint. El checklist de la plantilla de PR sale de aquí.

## Por ticket

Un ticket está terminado cuando se cumple todo esto:

- [ ] Compila sin errores y sin advertencias nuevas.
- [ ] Las pruebas pasan y el CI está en verde (check obligatorio `Build and test`).
- [ ] No hay secretos, contraseñas ni datos personales en el repositorio ni en los logs.
- [ ] La documentación está actualizada. Un PR que cambia comportamiento sin tocar la documentación no se fusiona.
- [ ] Todos los criterios de aceptación del ticket están cumplidos y verificados.
- [ ] Se fusionó por PR (squash) con `Closes #n`, y el issue quedó cerrado.
- [ ] El tablero muestra el ticket en **Done**.

### Cómo se comprueba

```
dotnet format PatriSystem.slnx --verify-no-changes
dotnet build PatriSystem.slnx --configuration Release -warnaserror
dotnet test PatriSystem.slnx
git grep --untracked -n -i "password"
```

El último comando solo debe mostrar nombres de variables y marcadores, nunca un valor real.

## Por sprint

### Sprint 0 (fundaciones)

- [ ] Los tickets **Must** están fusionados por PR con el CI en verde.
- [ ] `main` está protegido y la protección está probada.
- [ ] El proyecto se levanta desde cero siguiendo solo el README.
- [ ] No hay secretos ni contraseñas en el repositorio.
- [ ] El tablero tiene el backlog cargado.
- [ ] Se hizo la retrospectiva, con una mejora concreta para el Sprint 1.

### Sprints siguientes

La Definition of Done de cada sprint se define al iniciarlo, junto con el desglose de sus épicas en tickets.