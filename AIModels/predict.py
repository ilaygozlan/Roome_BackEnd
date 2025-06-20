import sys
from tensorflow.keras.applications.mobilenet import MobileNet, preprocess_input, decode_predictions
from tensorflow.keras.preprocessing import image
import numpy as np
import os

# Suppress TensorFlow logs
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'

# מילון תרגום - ריהוט בלבד
label_translations = {
    "studio_couch": "ספה",
    "sofa": "ספה",
    "armchair": "כורסה",
    "desk": "שולחן כתיבה",
    "dining_table": "שולחן אוכל",
    "wardrobe": "ארון בגדים",
    "bookshelf": "כוננית",
    "bed": "מיטה",
    "bathtub": "אמבטיה",
    "table_lamp": "מנורת שולחן",
    "television": "טלוויזיה",
    "tv": "טלוויזיה",
    "refrigerator": "מקרר",
    "oven": "תנור",
    "stove": "כיריים",
    "sink": "כיור",
    "washer": "מכונת כביסה",
    "dryer": "מייבש כביסה",
    "air_conditioner": "מזגן"
}

def predict(img_path):
    print(f"Predicting for: {img_path}")

    if not os.path.exists(img_path):
        return f"Error: File not found: {img_path}"

    print("Loading MobileNet model...")
    model = MobileNet(weights='imagenet')

    print("Loading and preprocessing image...")
    img = image.load_img(img_path, target_size=(224, 224))
    x = image.img_to_array(img)
    x = np.expand_dims(x, axis=0)
    x = preprocess_input(x)

    print("Performing prediction...")
    preds = model.predict(x)
    decoded = decode_predictions(preds, top=5)[0]

    top_labels = []
    for _, english_label, score in decoded:
        print(f"Detected: {english_label} ({score:.2f})")
        if score >= 0.40:
            english_label_clean = english_label.strip().lower()
            if english_label_clean in label_translations:
                hebrew_label = label_translations[english_label_clean]
                top_labels.append(hebrew_label)
            else:
                print(f"Ignored label: {english_label_clean}")

    print("Top labels (translated):", top_labels)
    return ';'.join(top_labels)

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python predict.py <image_path> [output_file]")
    else:
        try:
            result = predict(sys.argv[1])
            print(result)

            if len(sys.argv) == 3:
                with open(sys.argv[2], "w", encoding="utf-8") as f:
                    f.write(result)

        except Exception as e:
            print(e)