import requests

# === CONFIG ===
api_base = "http://localhost:5000/api/UploadImage"
apartment_id = 1  # שימי את ID של דירה קיימת אצלך במסד
image_path = "C:/Roome_BackEnd-Adi/Roome_BackEnd/wwwroot/uploadedFiles/b7c19d47-c8da-4273-9dde-bacd9d698a68.jpg"

# === STEP 1: Upload Image ===
upload_url = f"{api_base}/uploadApartmentImage/{apartment_id}"
files = {"files": open(image_path, "rb")}

print(f"Uploading image: {image_path}")
response = requests.post(upload_url, files=files)

if response.status_code == 200:
    data = response.json()
    print("\n✅ Image uploaded!")
    print("Detected Labels:", data.get("DetectedObjects"))
else:
    print("\n❌ Upload failed!")
    print("Status:", response.status_code)
    print("Response:", response.text)

# === STEP 2: Check saved labels (optional) ===
labels_url = f"{api_base}/GetImageLabelsByApartment/{apartment_id}"
summary_url = f"{api_base}/GetLabelSummaryByApartment/{apartment_id}"

print("\n📦 Getting stored labels...")
labels_res = requests.get(labels_url)
summary_res = requests.get(summary_url)

if labels_res.status_code == 200:
    print("Labels in DB:")
    for item in labels_res.json():
        print(f"- {item['ImagePath']} → {item['PredictedLabel']}")
else:
    print("⚠️ Failed to fetch labels")

if summary_res.status_code == 200:
    print("\nLabel Summary:")
    for item in summary_res.json():
        print(f"- {item['Label']} (x{item['Count']})")
