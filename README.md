# bobby-api

Serves a picture of the day. The same image is returned for a given UTC day,
cycling through the files in `images/`.

## Stack

ASP.NET Core on .NET 8.

## Quick start

```bash
dotnet restore
API_USERNAME=someone API_PASSWORD=secret dotnet run
```

Both variables are required. The process refuses to start without them.

## Environment

| Variable | Used for |
| --- | --- |
| `API_USERNAME`, `API_PASSWORD` | Basic auth, checked on every request |

## Endpoints

| Path | Auth |
| --- | --- |
| `/api/pictureoftheday` | Basic |
| `/health` | Anonymous |
| `/swagger` | Anonymous, and only mapped outside production |

Every other path requires Basic auth.

## Health

`/health` reports that the process is up, with no dependency checks. It is
exempt from the Basic auth middleware by exact path match, so `/health/anything`
still requires credentials.

## Deployment

Image [`sondresjo/bobby-api`](https://hub.docker.com/r/sondresjo/bobby-api) on
Docker Hub, chart `bobby-api` in
[tumogroup-charts](https://github.com/sondresjolyst/tumogroup-charts), applied by
Flux from [tumo-flux](https://github.com/sondresjolyst/tumo-flux) to
`bobby-prod`.

The container runs as the non-root `app` user. The chart pins `tag: latest`, and
the build only runs on a pushed tag, so a release-please release does not by
itself publish an image.

Cluster secrets are created by
[`scripts/bobby/bootstrap.sh`](https://github.com/sondresjolyst/tumo-platform/blob/main/scripts/bobby/bootstrap.sh)
in [tumo-platform](https://github.com/sondresjolyst/tumo-platform).

## License

Proprietary. Copyright (c) 2026 Sondre Sjølyst.
