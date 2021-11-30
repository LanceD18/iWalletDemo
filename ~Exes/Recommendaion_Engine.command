#!/usr/bin/env python
# coding: utf-8

# # Item Recommendations
# 
# System will compare the similarity ratings using Jaccard Similairty and Doc2Vec and reccomend an item with the highest similarity rating from the two methods.

# ## Sample Data

# In[1]:

import os
os.system('pip3 install virtualenv')
os.system('virtualenv ENV')
os.system('ENV/bin/activate')
os.system('pip3 install pandas')
os.system('pip3 install gensim')
os.system('pip3 install nltk')
os.system('pip3 install IPython')
os.system('pip3 install ipywidgets')
#subprocess.check_call(['pip3', 'install', '<pandas>'])
#subprocess.check_call(['pip3', 'install', '<gensim>'])
#subprocess.check_call(['pip3', 'install', '<nltk>'])
#subprocess.check_call(['pip3', 'install', '<IPython>'])
#subprocess.check_call(['pip3', 'install', '<ipywidgets>'])


import pandas as pd
import numpy as np


# In[2]:


user_schedule_dict = {
    'Date': ['11/07/2021', '11/07/2021', '11/06/2021'],
    'Time': [12, 8, 13],
    'Place': ['Taco Bell', 'School', 'Gym'], 
    'Item': ['Credit Card', 'School ID', 'Gym Pass'],
    'Description': ['Buy 4 burriots from taco bell using credit',
                     'Swipped ID to get into study room at school',
                     'Swipped ID to get into gym']
}

items_dict = {
    'Items': ['Credit Card', 'School ID', 'Gym Pass', 'Debit Card',
             'Concert Ticket', 'Bus Pass'],
    'Description': ['Used to buy things using credit',
                    'Used to access rooms and buildings at school',
                    'Used to get into the gym',
                    'Used to buy things using debit',
                    'Used to enter a concert',
                    'Used to get on the bus']
}

# Convert dictionaries to data frames
user_schedule = pd.DataFrame(user_schedule_dict)
user_items = pd.DataFrame(items_dict)


# In[3]:


# View user_schedule df
user_schedule


# In[4]:


# View user_items df
user_items


# ## 1. Jaccard Similarity
# 
# Will use Jaccard Similarity to find a similarity score between the user's activity schedule and an item's description and recommend that the user use the item with the highest score

# In[5]:


def Jaccard_Similarity(doc1, doc2): 
    
    # List the unique words in a document
    words_doc1 = set(doc1.lower().split()) 
    words_doc2 = set(doc2.lower().split())
    
    # Find the intersection of words list of doc1 & doc2
    intersection = words_doc1.intersection(words_doc2)

    # Find the union of words list of doc1 & doc2
    union = words_doc1.union(words_doc2)
        
    # Calculate Jaccard similarity score 
    # using length of intersection set divided by length of union set
    return float(len(intersection)) / len(union)


# Loop through the user_schedule and user_items data frames and calculate the similarity scores between every item/description in the user_schedule vs every item/description in the user_items.
# 
# The final similarity score of any particualr item is the summation of the similarity scores for that item

# In[6]:


similarity_scores = {}
for i, sch_item in user_schedule.iterrows():
    for j, item in user_items.iterrows():
        jacc = Jaccard_Similarity(item['Description'], sch_item['Description'])
        similarity_scores[item['Items']] = similarity_scores.get(item['Items'], 0) + jacc


# View similarity_scores dictionary

# In[7]:


similarity_scores


# ## 2. Doc2Vec
# 
# Represent documents as numbers and outputs a similarity score.


from gensim.test.utils import common_texts
from gensim.models.doc2vec import Doc2Vec, TaggedDocument


# Create a user_descriptions list which has all of the descriptions from their schedule and from their wallet.
# 
# Create a docuemtns list which is an indexed/tagged version of the user_descriptions list.
# 
# Instantiate the Doc2Vec model using out documents list.

# In[11]:


user_descriptions = list(user_schedule['Description']) + list(user_items['Description'])
documents = [TaggedDocument(doc, [i]) for i, doc in enumerate(user_descriptions)]
model = Doc2Vec(documents, vector_size=5, window=2, min_count=1, workers=4)


# View documents

# In[12]:


documents


# Discover the set of all possible words/doc-tags – and in the case of words, finds which words occur more than min_count times.

# In[13]:


model.build_vocab(documents)


# Train Doc2Vec model based on the documents list

# In[14]:


model.train(documents, total_examples = model.corpus_count, epochs = 80)


# Save the model

# In[15]:


model.save("d2v.model")


# Load the model

# In[16]:


model = Doc2Vec.load("d2v.model")


# We want to calcualte the similarities between of indexes [3,4] against [0, 2]

# In[17]:


similar_doc = model.docvecs.most_similar(0)
print(similar_doc[0])

similar_doc = model.docvecs.most_similar(1)
print(similar_doc[1])

similar_doc = model.docvecs.most_similar(2)
print(similar_doc[2])


# # Recommendations
# 
# Student ID and Gym Pass have the highest scores. Recommendations were done using **Doc2Vec and Jaccard**

# # User acceptance of recomendation

# In[18]:


from IPython.display import display
import ipywidgets as widgets


# In[19]:


# On-click event for accept button
def accepted (arg):
    acceptance = True
    print('Recommendation Accepted. Acceptance saved.')

# On-click event for reject button
def rejected (arg):
    acceptance = False
    print('Recommendation Rejected. Acceptance saved.')


# In[20]:


# Define accept and reject buttons
accept = widgets.Button(description = 'Accept')
reject = widgets.Button(description = 'Reject')

# Define on-click events for accept and reject buttons
accept.on_click(accepted)
reject.on_click(rejected)


# In[21]:


# Get user input
print('Accept or Reject recommendation.')
display(widgets.HBox((accept, reject)))


# # User rating for the recomendation usefulness (1-5 Stars)

# In[22]:


rec_rating = ''

# On-click event to rate recommendation
def rate (rating_stars):
    rec_rating = rating_stars.description
    print('Recommendation rated as ' + str(rec_rating) + '. Rating saved.')


# In[23]:


# Define rating buttons
star_1 = widgets.Button(description = '1')
star_2 = widgets.Button(description = '2')
star_3 = widgets.Button(description = '3')
star_4 = widgets.Button(description = '4')
star_5 = widgets.Button(description = '5')

# Define on-click events for rating buttons
star_1.on_click(rate)
star_2.on_click(rate)
star_3.on_click(rate)
star_4.on_click(rate)
star_5.on_click(rate)


# In[24]:


# Get user input
print('Choose a rating of the recommendation (1-5 Stars)')
display(widgets.HBox((star_1, star_2, star_3, star_4, star_5)))

