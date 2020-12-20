db = db.getSiblingDB('FacebookProducerDb');
db.createUser(
  {
    user: 'user',
    pwd: 'user',
    roles: [{ role: 'readWrite', db: 'FacebookProducerDb' }],
  },
);

db = db.getSiblingDB('TwitterProducerDb');
db.createUser(
  {
    user: 'user',
    pwd: 'user',
    roles: [{ role: 'readWrite', db: 'TwitterProducerDb' }],
  },
);

db = db.getSiblingDB('FeedsProducerDb');
db.createUser(
  {
    user: 'user',
    pwd: 'user',
    roles: [{ role: 'readWrite', db: 'FeedsProducerDb' }],
  },
);