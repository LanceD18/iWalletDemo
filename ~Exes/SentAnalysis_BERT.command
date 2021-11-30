################################################
# Bert Model with Tranfsormers                 #
# pip3 install torch torchvision torchaudio     #
# pip3 install transformers                    #
################################################

from transformers import AutoTokenizer, AutoModelForSequenceClassification
import torch

# Instantiate Model
tokenizer = AutoTokenizer.from_pretrained('nlptown/bert-base-multilingual-uncased-sentiment')
model = AutoModelForSequenceClassification.from_pretrained('nlptown/bert-base-multilingual-uncased-sentiment')

# Encode and Calculate Sentiment
tokens = tokenizer.encode('The app was slow while I was getting a recommendation', return_tensors = 'pt')
result = model(tokens)

# Calculate sentiment. Need +1 because we will have a 0-based index returned to us, +1 will get us the star value
sentiment = int(torch.argmax(result.logits)) + 1

# Display sentiment
print(sentiment)
