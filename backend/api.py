from fastapi import FastAPI, HTTPException

import openai
import uvicorn
import os
import re
import requests
from PIL import Image
from fastapi import FastAPI, File, Response
import io




app = FastAPI()
player_stories = {}
# get api from environment variable
# openai.api_key = os.environ['OPENAI_API_KEY']
openai.api_key  = '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'
# Replace 'YOUR_API_KEY' with your actual Pexels API key
# pexel_api_key = os.environ['PEXELS_API_KEY']

pexel_api_key = '34'


def split_paragraph(paragraph, chunk_size):
    words = paragraph.split()
    chunks = []
    current_chunk = ""

    for word in words:
        if len(current_chunk) + len(word) <= chunk_size:
            current_chunk += word + " "
        else:
            chunks.append(current_chunk.strip())
            current_chunk = word + " "
    
    if current_chunk:
        chunks.append(current_chunk.strip())

    return chunks

def get_options(text):
    # start with '}}' end end with \n
    
    lines = text.split('\n')
    options = []

    for line in lines:
        line = line.strip()
        if line.startswith('{{') and '}}' in line:
            option = line[line.index('}}') + 2:].strip()
            options.append(option)
    
    return options


@app.get("/")
def read_root():
    return {"HackUPC 2023": "Welcome to our HackUPC 2023 project!"}


@app.get("/start")
def start_game(player_id: int):
    global player_stories
    story = player_stories.get(player_id, "")

    if story:
        return {
            "Text": [],
            "Options": [],
        }
    
    completion = openai.ChatCompletion.create(
    model="gpt-3.5-turbo",
    messages=[
            {"role": "system", "content": "You are an engaging storyteller, the story would be set in Barcelona, it would include all the popular places in Barcelona. You will come up with entertaining stories that are engaging, and has immersive choices for the user. Genre of the story can be thriller, suspense or historical fiction.\
            Your response should be in the format of\
            AI - Here you will give buildup for a cohesive story. (Dont make it too long) \
            \
            After the buildup You Will always ask a question and give immersive situations or answers as choices to choose from so that the user can choose and a story can be build around it.\
            Each choices should be less than 3 words \n \
             Example:\
            {{1}} Choice  \n \
            {{2}} Choice  \n \
            {{3}} Choice  \n \
            The number of choices ideally should be 3 but can be upto 5.\
            \
            Wait for the user to choose an option. \
            \
            My first request is “Start the immersive choice based story for me based in barcelona."},
                    ],
                    stop=["User", "User"],
            )

    start_text = completion.choices[0].message["content"]
    story += f"\n{start_text}\n"
    # get text after AI - and '{{' from start_text
    start_text_clean = start_text[: start_text.find("{{")].strip()
    # remove if start_text_clean contains 'AI -'
    if 'AI' in start_text_clean:
        start_text_clean = start_text_clean.replace('AI', '')
    # split start_text_clean into 299 character chunks with full words
    start_text_chunks = split_paragraph(start_text_clean, 299)
    options = get_options(start_text)
    return {
        "Text": start_text_chunks,
            "Options": options
            }



@app.get("/generate")
def generate_story(player_id: int, player_input: str):
    global player_stories
    
    story = player_stories.get(player_id, "")
    
    if not story:
        return {
            "Text": [],
            "Options": [],
        }
    
    # Call the OpenAI Chat GPT API and generate the next part of the story based on the player's input
    completion = openai.ChatCompletion.create(
        model="gpt-3.5-turbo",
        messages=[
            {"role": "assistant", "content": story},
            {"role": "user", "content": player_input},
            {"role": "assistant", "content": "Here you will give buildup for a cohesive story. (Dont make it too long) \
            After the buildup You Will always ask a immersive question and give immersive choices so that the user can choose and a story can be build around it.\
            Each choices should be less than 3 words \n \
             Example:\
            {{1}} Choice  \n \
            {{2}} Choice  \n \
            {{3}} Choice  \n \
            The number of choices ideally should be 3 but can be upto 5.\
            Wait for the user to choose an option."}
        ],
    )
    
    # Extract the generated text from the API response
    generated_text = completion.choices[0].message["content"]
    
    # Update the story based on the generated text and the player's input
    story += f"\nUser chose {player_input}\nAI- {generated_text}\n"
    # get the line after User chose Num using regex
    generated_text_ai_part = generated_text[ : generated_text.find("{{")].strip()
    # split generated_text into 299 character chunks with full words
    generated_text_chunks = split_paragraph(generated_text_ai_part, 299)
    options = get_options(generated_text)

    player_stories[player_id] = story
    
    return {
        "Text": generated_text_chunks,
        "Options": options
    }


def get_image_url(place_name, api_key):
    headers = {
        'Authorization': api_key
    }
    params = {
        'query': place_name,
        'per_page': 1
    }
    url = 'https://api.pexels.com/v1/search'
    
    response = requests.get(url, headers=headers, params=params)
    
    if response.status_code == 200:
        data = response.json()
        if data['total_results'] > 0:
            photo = data['photos'][0]
            image_url = photo['src']['large']
            return image_url
    
    return None

def resize_image(image_path, width, height):
    # Open the image
    image = Image.open(image_path)
    
    # Resize the image
    resized_image = image.resize((width, height))
    
    return resized_image

def convert_to_binary(image):
    # Convert the image to grayscale
    grayscale_image = image.convert('L')
    
    # Convert the grayscale image to binary (black and white)
    binary_image = grayscale_image.point(lambda x: 0 if x < 128 else 255, '1')
    
    return binary_image


# create a new endpoint with image search which would have a query


@app.get("/image")
async def image_search(query: str):
    
    place_name = query
    image_url = get_image_url(place_name, pexel_api_key)
    if image_url:
        response = requests.get(image_url)
        image_path = 'image.jpg'

        # Save the retrieved image
        with open(image_path, 'wb') as f:
            f.write(response.content)

        # Specify the desired size
        width = 380
        height = 270

        # Resize the image
        resized_image = resize_image(image_path, width, height)

        # Convert the resized image to binary
        binary_image = convert_to_binary(resized_image)

        # Save the binary image
        binary_image_path = '/tmp/binary_image.jpg'
        binary_image.save(binary_image_path)

        # Return the URL of the binary image for download
        binary_image_url = f"http://hack-upc-backend-api.vercel.app/download?file={binary_image_path}"
        return {"image_url": binary_image_url}

    return {"error": "Image not found."}

@app.get("/download")
def download_image(file: str):
    # Check if the binary image file exists
    if os.path.exists(file):
        # Set the appropriate headers for downloading the file
        headers = {
            "Content-Disposition": f"attachment; filename={file}"
        }
        # Open the file in binary mode
        with open(file, "rb") as f:
            contents = f.read()
        # Return the file as a response with appropriate headers
        return Response(content=contents, headers=headers, media_type="image/jpeg")
    else:
        return {"error": "File not found."}




if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8000)
