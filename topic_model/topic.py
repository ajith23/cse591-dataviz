from gensim import corpora, models, similarities
from gensim.models import hdpmodel, ldamodel
from itertools import izip
import csv
import sys

# documents = ["Human machine interface for lab abc computer applications",
#               "A survey of user opinion of computer system response time",
#               "The EPS user interface management system",
#               "System and human system engineering testing of EPS",
#               "Relation of user perceived response time to error measurement",
#               "The generation of random binary unordered trees",
#               "The intersection graph of paths in trees",
#               "Graph minors IV Widths of trees and well quasi ordering",
#               "Graph minors A survey"]

documents = []
i=0
with open('data.csv') as csvfile:
	reader = csv.DictReader(csvfile)
	for row in reader:
		tag = row['Tags'].replace('<','').split('>')
		if 'python' in tag:
			i = i+1
			print i
			if i <100:
				print i,row['Title']
				documents.append(row['Title'].replace('?',''))
			else:
				break
# else:
# break

print documents
print len(documents)
sys.exit(0)
# remove common words and tokenize
stoplist = set('does most  your string it with another an from how what why when where is can which are be i should for a of the and to in'.split())
texts = [[word for word in document.lower().split() if word not in stoplist]
         for document in documents]


# remove words that appear only once
all_tokens = sum(texts, [])
tokens_once = set(word for word in set(all_tokens) if all_tokens.count(word) == 1)
texts = [[word for word in text if word not in tokens_once]
         for text in texts]

dictionary = corpora.Dictionary(texts)
# print dictionary
corpus = [dictionary.doc2bow(text) for text in texts]
# print corpus
# # I can print out the topics for LSA
# lsi = models.LsiModel(corpus_tfidf, id2word=dictionary, num_topics=2)
# corpus_lsi = lsi[corpus]

# for l,t in izip(corpus_lsi,corpus):
#   print l,"#",t
# print
# for top in lsi.print_topics(2):
#   print top

# I can print out the documents and which is the most probable topics for each doc.
lda = ldamodel.LdaModel(corpus, id2word=dictionary, num_topics=10)
# corpus_lda = lda[corpus]

# for l,t in izip(corpus_lda,corpus):
#   print l,"#",t
# # print

# But I am unable to print out the topics, how should i do it?
print lda.print_topics(10)
for w,p in lda.show_topic(1,20):
  print w, p