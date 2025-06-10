# Bobby API

Bobby API serves a "picture of the day" by rotating through images in a folder. The same image is returned for a given UTC day, cycling through all available images.

## Features

- Returns a base64-encoded image, its filename, and content type.
- Rotates the image daily based on the current UTC day of the year.
- Supports `.jpg`, `.jpeg`, and `.png` images.
- Simple HTTP Basic Authentication using environment variables or user secrets.

## How Image Selection Works

1. The API lists all images in the `images` folder.
2. It calculates the current day of the year (UTC).
3. Selects an image using:  
   `index = dayOfYear % number_of_images`

**Example Calculation:**

| dayOfYear | number_of_images | index |
|-----------|------------------|-------|
| 15        | 10               | 5     |
| 16        | 10               | 6     |
| 17        | 10               | 7     |

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/get-started/) (optional, for containerization)

### Setup

1. **Add Images**

   Place your images in the `images` folder at the project root. Supported formats: `.jpg`, `.jpeg`, `.png`.

2. **Set Authentication Secrets**

   Set the following environment variables or [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

   - `API_USERNAME`
   - `API_PASSWORD`

3. **Run the API**

   You can run the API using the .NET CLI:
   ```bash
   dotnet run
   ```
   Or if you prefer Docker:
   ```bash
   docker build -t bobby-api .
   docker run -p 7297:7297 -t bobby-api -e API_USERNAME=<username> -e API_PASSWORD=<password>
   ```

4. **Access the API**
   Open your browser or use a tool like `curl` to access the API:
   ```
   http://localhost:7297/PictureOfTheDay
   ```
   You will need to provide the username and password set in the environment variables for authentication.

## Environment Variables

| Name         | Description                | Required |
|--------------|---------------------------|----------|
| API_USERNAME | Username for authentication| Yes      |
| API_PASSWORD | Password for authentication| Yes      |