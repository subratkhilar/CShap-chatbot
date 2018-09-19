var express = require('express'),

  https = require('https'),
http  = require('http'),
  router = express.Router(),

  compression = require('compression'),

  path = require('path');

//swaggerUi = require('swagger-ui-express'),

  //swaggerDocument = require('./swagger.json');

  var constants = require('constants');

var request = require('request');

var btoa = require("btoa");

var apicache = require('apicache');

 let bodyParser = require('body-parser');
 
/* 
var fs = require('fs')

  , path = require('path')

  , certFile = path.resolve(__dirname, 'ssl/udit.crt')

  , keyFile = path.resolve(__dirname, 'ssl/udit.key')

  , caFile = path.resolve(__dirname, 'ssl/ca.crt');

 

var https_options = {

  cert: fs.readFileSync(certFile),

  key: fs.readFileSync(keyFile),

  passphrase: 'wasadmin',

  ca: fs.readFileSync(caFile),

  agent:false,

  rejectUnauthorized:false,

  secureOptions: constants.SSL_OP_NO_TLSv1_2,

  strictSSL:false

}; */

 

 

 

//var incident_Url="https://dev48452.service-now.com/api/now/table/incident"
var incident_Url="https://dev56254.service-now.com/api/now/table/incident";

 

 

// HTTP API

 

const app = express();

app.use(compression());
app.use(bodyParser.json());
 

const port = process.env.PORT || 3019;

 

const options = {

  url: "",

  time: true,

  headers: {

    'Authorization': 'Basic ' + btoa('admin' + ':' + 'Subk!@20167'),

    'Content-Type': 'application/json',

    'Accept': 'application/json',

  },

 

};

 

 

app.use((req, res, next) => {

  res.header('Access-Control-Allow-Origin', "*");

  res.header('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, HEAD');

  res.header('Access-Control-Allow-Credentials', "true");

  res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept, Authorization, X-Client-Id, X-Access-Token');

 

  if (req.method == 'OPTIONS') {

    res.sendStatus(200);

  } else {

    next();

  }

});

 

let cache = apicache.middleware;

 

//apicache.options({debug: true});

 

//app.use(cache('1 day'));


app.get('/api/v1/getIncidentDetails', function (req, res) {

  var getChangeOptions = JSON.parse(JSON.stringify(options));

  getChangeOptions.url = incident_Url;

  request.get(getChangeOptions, (err, response, body) => {

    if (response) {

      //res.status(200).json(body);
var obj = JSON.parse(body);
var objArry = obj.result;

res.status(200).json(objArry[0].number);
//console.log(objArry[0].number);
                 //console.log("sucess>> ");
				// console.log(body.result);
				 //console.log(response);
				 //console.log("====================");

    }

    else {

      res.json(err);

                 console.log(err)

      //apicache.clear('/api/v1/getIncidentDetails');

    }

  });

})

 
 app.get('/api/v1/getIncidentDetailsTest/{incidentNo}', function (req, res) {

  var getChangeOptions = JSON.parse(JSON.stringify(options));
var incNumber = req.param.incidentNo;
console.log("incident No"+incNumber);
  getChangeOptions.url = incident_Url+"?number=INC0000001";

  request.get(getChangeOptions, (err, response, body) => {

    if (response) {

      //res.status(200).json(body);
var obj = JSON.parse(body);
var objArry = obj.result;

res.status(200).json(body);
//console.log(objArry[0].number);
                 //console.log("sucess>> ");
				// console.log(body.result);
				 //console.log(response);
				 //console.log("====================");

    }

    else {

      res.json(err);

                 console.log(err)

      //apicache.clear('/api/v1/getIncidentDetails');

    }

  });

})

 

 

 


 

 

app.post('/api/v1/incident', function (req, res) {

              // console.log(req);


  var getChangeOptions = JSON.parse(JSON.stringify(options));

  getChangeOptions.url = incident_Url;

  request.post(getChangeOptions, (err, response, body) => {

                 console.log(req.body);

    if (response) {

      //return res.status(201).json(body);
	 var obj = JSON.parse(body);
//var objArry = obj.result;

res.status(200).json(obj.result.number);

    }

    else {

      res.json(err);

      //apicache.clear('/api/v1/getIncidentDetails');

    }

  });

})

 

app.post('/api/v1/incidentTest', function (req, res) {

request.post({

    "headers": { 'Authorization': 'Basic ' + btoa('sys_rest_diana' + ':' + 'BYBK39jq'),

    'Content-Type': 'application/json',

    'Accept': 'application/json'},

    "url": incident_Url,

    "body": JSON.stringify({

          "caller_id":"pgont",

                 "assignment_group":"FS_TECHCAFE-AI-BLR_SUP",

                 "contact_type":"self-service",

                 "category":"Investigation",

                 "short_description":"Software issue - XXX not working",

                 "cmdb_ci":"MyATSC Incident Mgmt",

                 "description":"Not able to install new software"

    })

}, (error, response, body) => {

    if(error) {

        return console.dir(error);

    }

    console.dir(JSON.parse(body));

});

})

 

 

//https.createServer(https_options, app).listen(port);
http.createServer(app).listen(port);

console.log('info', '[EXPRESS] - listening port: ', port);