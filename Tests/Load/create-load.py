import os
import asyncio
import aiohttp

FILES = ["file1.pdf", "file2.pdf", "file3.pdf"]
API_URL = "https://localhost:8082/api/tool/test"

TOTAL_REQUESTS = 10000
CONCURRENT_REQUESTS_LIMIT = 100

semaphore = asyncio.Semaphore(CONCURRENT_REQUESTS_LIMIT)

async def send_merge_request(session):
    data = aiohttp.FormData()

    for file_path in FILES:
        file_name = os.path.basename(file_path)

        data.add_field(
            'files',
            open(file_path, 'rb'),
            filename=file_name,
            content_type='application/pdf'
        )

    async with session.post(API_URL, data=data) as response:
        return response.status

async def task(session, request_number):
    async with semaphore:
        try:
            status = await send_merge_request(session)
            print(f"Request {request_number} sent: {status}")
        except Exception as e:
            print(f"Request {request_number} failed: {e}")

async def main():
    conn = aiohttp.TCPConnector(
        ssl=False,
        limit=CONCURRENT_REQUESTS_LIMIT
    )

    async with aiohttp.ClientSession(connector=conn) as session:
        tasks = [asyncio.create_task(task(session, i)) for i in range(TOTAL_REQUESTS)]
        await asyncio.gather(*tasks)

    print("All requests finished")

if __name__ == "__main__":
    asyncio.run(main())
