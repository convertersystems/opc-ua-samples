/* eslint no-process-exit: 0 */
"use strict";
require("requirish")._(module);
Error.stackTraceLimit = Infinity;

var opcua = require("./node_modules/node-opcua");
var _ = require("underscore");
var path = require("path");
var fs = require("fs");
var assert = require("assert");
var argv = require('yargs').alias('a', 'allowAnonymous').argv;

var OPCUAServer = opcua.OPCUAServer;
var Variant = opcua.Variant;
var DataType = opcua.DataType;
var DataValue = opcua.DataValue;
var QualifiedName = opcua.QualifiedName;
var get_fully_qualified_domain_name = opcua.get_fully_qualified_domain_name;
var makeApplicationUrn = opcua.makeApplicationUrn;
var standard_nodeset_file = opcua.standard_nodeset_file;
var StatusCodes = opcua.StatusCodes;
var makeNodeId = opcua.makeNodeId;

var path = require("path");
var pjson = require('./package.json');
var homedir = require("os").homedir();
var appdatalocal = path.join(homedir, "/AppData/Local/", pjson.name);
var server_certificate_file = "node_modules/node-opcua/certificates/server_selfsigned_cert_2048.pem";
var server_certificate_privatekey_file = "node_modules/node-opcua/certificates/server_key_2048.pem";

var port = parseInt(pjson.config.port) || 26543;

var userManager = {
    isValidUser: function (userName, password) {
       console.log("  Validating user".yellow);
       console.log("    userName: ".yellow, userName);
       console.log("    password: ".yellow, password);
       return true;
    }
};

var server_options = {
    certificateFile: server_certificate_file,
    privateKeyFile: server_certificate_privatekey_file,
    port: port,
    maxAllowedSessionNumber: 500,
    nodeset_filename: [
        standard_nodeset_file,
        path.join(__dirname, "/PredefinedNodes.xml"),

    ],
    serverInfo: {
        applicationUri: makeApplicationUrn(get_fully_qualified_domain_name(), pjson.name),
        productUri: pjson.homepage,
        applicationName: { text: pjson.name, locale: "en" },
        gatewayServerUri: null,
        discoveryProfileUri: null,
        discoveryUrls: []
    },
    buildInfo: {
        productName: pjson.name,
        productUri: pjson.homepage,
        manufacturerName: pjson.author.name,
        softwareVersion: pjson.version,
    },
    serverCapabilities: {
        operationLimits: {
            maxNodesPerRead: 1000,
            maxNodesPerBrowse: 2000
        }
    },
    userManager: userManager,
    allowAnonymous: argv.allowAnonymous ? true : false,
    isAuditing: true
};

process.title = pjson.name + " on port : " + server_options.port;

var server = new OPCUAServer(server_options);

var endpointUrl = server.endpoints[0].endpointDescriptions()[0].endpointUrl;

var hostname = require("os").hostname();

var _axis1 = 0.0;
var _axis2 = 0.0;
var _axis3 = 0.0;
var _axis4 = 0.0;
var _mode = 2;
var _speed = 0;
var _laser = false;
var _timestamp = new Date();
var _masterAxis = 0.0;

server.on("post_initialize", function () {
    var addressSpace = server.engine.addressSpace;
    var nsi = addressSpace.getNamespaceIndex("https://github.com/ConverterSystems/UaClient");

    var _robot1 = addressSpace.findNode(makeNodeId("Robot1", nsi));

    var _axis1Variable = addressSpace.findNode(makeNodeId("Robot1_Axis1", nsi));
    _axis1Variable.bindVariable({
        timestamped_get: function () {
            return new DataValue({
                value: { dataType: DataType.Float, value: _axis1 },
                statusCode: StatusCodes.Good,
                sourceTimestamp: _timestamp
            });
        },
        set: function (val) {
            _axis1 = val.value;
            return StatusCodes.Good;
        }
    });

    var _axis2Variable = addressSpace.findNode(makeNodeId("Robot1_Axis2", nsi));
    _axis2Variable.bindVariable({
        timestamped_get: function () {
            return new DataValue({
                value: { dataType: DataType.Float, value: _axis2 },
                statusCode: StatusCodes.Good,
                sourceTimestamp: _timestamp
            });
        },
        set: function (val) {
            _axis2 = val.value;
            return StatusCodes.Good;
        }
    });

    var _axis3Variable = addressSpace.findNode(makeNodeId("Robot1_Axis3", nsi));
    _axis3Variable.bindVariable({
        timestamped_get: function () {
            return new DataValue({
                value: { dataType: DataType.Float, value: _axis3 },
                statusCode: StatusCodes.Good,
                sourceTimestamp: _timestamp
            });
        },
        set: function (val) {
            _axis3 = val.value;
            return StatusCodes.Good;
        }
    });

    var _axis4Variable = addressSpace.findNode(makeNodeId("Robot1_Axis4", nsi));
    _axis4Variable.bindVariable({
        timestamped_get: function () {
            return new DataValue({
                value: { dataType: DataType.Float, value: _axis4 },
                statusCode: StatusCodes.Good,
                sourceTimestamp: _timestamp
            });
        },
        set: function (val) {
            _axis4 = val.value;
            return StatusCodes.Good;
        }
    });

    var _modeVariable = addressSpace.findNode(makeNodeId("Robot1_Mode", nsi));
    _modeVariable.bindVariable({
        timestamped_get: function () {
            return new DataValue({
                value: { dataType: DataType.Int16, value: _mode },
                statusCode: StatusCodes.Good,
                sourceTimestamp: _timestamp
            });
        },
        set: function (val) {
            _mode = parseInt(val.value);
            _robot1.raiseEvent("BaseEventType", {
                message: {
                    dataType: DataType.LocalizedText,
                    value: { text: _mode === 0 ? "Mode to Off." : _mode === 1 ? "Mode to Hand." : "Mode to Auto." }
                },
                severity: {
                    dataType: DataType.UInt16,
                    value: 500
                }
            });
            return StatusCodes.Good;
        }
    });

    var _speedVariable = addressSpace.findNode(makeNodeId("Robot1_Speed", nsi));
    _speedVariable.bindVariable({
        timestamped_get: function () {
            return new DataValue({
                value: { dataType: DataType.Int16, value: _speed },
                statusCode: StatusCodes.Good,
                sourceTimestamp: _timestamp
            });
        },
        set: function (val) {
            _speed = val.value;
            return StatusCodes.Good;
        }
    });

    var _laserVariable = addressSpace.findNode(makeNodeId("Robot1_Laser", nsi));
    _laserVariable.bindVariable({
        timestamped_get: function () {
            return new DataValue({
                value: { dataType: DataType.Boolean, value: _laser },
                statusCode: StatusCodes.Good,
                sourceTimestamp: _timestamp
            });
        },
        set: function (val) {
            _laser = val.value;
            _robot1.raiseEvent("BaseEventType", {
                message: {
                    dataType: DataType.LocalizedText,
                    value: { text: _laser ? "Laser activated." : "Laser deactivated." }
                },
                severity: {
                    dataType: DataType.UInt16,
                    value: 500
                }
            });
            return StatusCodes.Good;
        }
    });

    var _multiplyMethod = addressSpace.findNode(makeNodeId("Robot1_Multiply", nsi));
    _multiplyMethod.bindMethod(function (inputArguments, context, callback) {
        var a = inputArguments[0].value || 0.0;
        var b = inputArguments[1].value || 0.0;
        var callMethodResult = {
            statusCode: StatusCodes.Good,
            outputArguments: [
                { dataType: DataType.Double, value: a * b }
            ]
        };
        callback(null, callMethodResult);
    });


    var _stopMethod = addressSpace.findNode(makeNodeId("Robot1_Stop", nsi));
    _stopMethod.bindMethod(function (inputArguments, context, callback) {
        _mode = 0;
        context.object.raiseEvent("BaseEventType", {
            message: {
                dataType: DataType.LocalizedText,
                value: { text: "Mode to Off." }
            },
            severity: {
                dataType: DataType.UInt16,
                value: 100
            }
        });
        var callMethodResult = {
            statusCode: StatusCodes.Good,
            outputArguments: []
        };
        callback(null, callMethodResult);
    });

    setInterval(function () {
        var now = new Date();
        var dt = now - _timestamp;
        _timestamp = now;

        if (_mode !== 1) // if not in man mode
        {
            var period = 30000;
            switch (_speed) {
                case 1:
                    period = 20000;
                    break;
                case 2:
                    period = 10000;
                    break;
                case 3:
                    period = 5000;
                    break;
                default:
                    period = 30000;
                    break;
            }
            if (_mode === 2) // auto mode
            {
                _masterAxis = (_masterAxis + dt / period) % 1.0; // 0.0 to 1.0
            }
            _axis1 = Math.sin(_masterAxis * 2.0 * Math.PI) * 45.0;
            _axis2 = Math.cos(_masterAxis * 2.0 * Math.PI) * 45.0;
            _axis3 = Math.sin(((_masterAxis * 2.0) % 1.0) * 2.0 * Math.PI) * 45.0;
            _axis4 = Math.cos(_masterAxis * 2.0 * Math.PI) * -180.0;
        }
    }, 250);
});


function dumpObject(obj) {
    function w(str, width) {
        var tmp = str + "                                        ";
        return tmp.substr(0, width);
    }

    return _.map(obj, function (value, key) {
        return "      " + w(key, 30).green + "  : " + ((value === null) ? null : value.toString());
    }).join("\n");
}


console.log("  server PID          :".yellow, process.pid);

server.start(function (err) {
    if (err) {
        console.log(" Server failed to start ... exiting");
        process.exit(-3);
    }
    console.log("  server on port      :".yellow, server.endpoints[0].port.toString().cyan);
    console.log("  endpointUrl         :".yellow, endpointUrl.cyan);

    console.log("  serverInfo          :".yellow);
    console.log(dumpObject(server.serverInfo));
    console.log("  buildInfo           :".yellow);
    console.log(dumpObject(server.engine.buildInfo));

    console.log("\n  server now waiting for connections. CTRL+C to stop".yellow);

});

server.on("create_session", function (session) {

    console.log(" SESSION CREATED");
    console.log("    client application URI: ".cyan, session.clientDescription.applicationUri);
    console.log("        client product URI: ".cyan, session.clientDescription.productUri);
    console.log("   client application name: ".cyan, session.clientDescription.applicationName.toString());
    console.log("   client application type: ".cyan, session.clientDescription.applicationType.toString());
    console.log("              session name: ".cyan, session.sessionName ? session.sessionName.toString() : "<null>");
    console.log("           session timeout: ".cyan, session.sessionTimeout);
    console.log("                session id: ".cyan, session.sessionId);
});

server.on("session_closed", function (session, reason) {
    console.log(" SESSION CLOSED :", reason);
    console.log("              session name: ".cyan, session.sessionName ? session.sessionName.toString() : "<null>");
});

function w(s, w) {
    return ("000" + s).substr(-w);
}
function t(d) {
    return w(d.getHours(), 2) + ":" + w(d.getMinutes(), 2) + ":" + w(d.getSeconds(), 2) + ":" + w(d.getMilliseconds(), 3);
}

server.on("response", function (response) {

    console.log(t(response.responseHeader.timeStamp), response.responseHeader.requestHandle,
        response._schema.name.cyan, " status = ", response.responseHeader.serviceResult.toString().cyan);
});

function indent(str, nb) {
    var spacer = "                                             ".slice(0, nb);
    return str.split("\n").map(function (s) {
        return spacer + s;
    }).join("\n");
}
server.on("request", function (request, channel) {
    console.log(t(request.requestHeader.timeStamp), request.requestHeader.requestHandle,
        request._schema.name.yellow, " ID =", channel.secureChannelId.toString().cyan);
});

process.on('SIGINT', function () {
    // only work on linux apparently
    console.log(" Received server interruption from user ".red.bold);
    console.log(" shutting down ...".red.bold);
    server.shutdown(1000, function () {
        console.log(" shutting down completed ".red.bold);
        console.log(" done ".red.bold);
        console.log("");
        process.exit(-1);
    });
});

var discovery_server_endpointUrl = "opc.tcp://" + hostname + ":4840/UADiscovery";

console.log("\nregistering server to :".yellow + discovery_server_endpointUrl);

server.registerServer(discovery_server_endpointUrl, function (err) {
    if (err) {
        // cannot register server in discovery
        console.log("     warning : cannot register server into registry server".cyan);
    } else {

        console.log("     registering server to the discovery server : done.".cyan);
    }
    console.log("");
});


server.on("newChannel", function (channel) {
    console.log("Client connected with address = ".bgYellow, channel.remoteAddress, " port = ", channel.remotePort);
});
server.on("closeChannel", function (channel) {
    console.log("Client disconnected with address = ".bgCyan, channel.remoteAddress, " port = ", channel.remotePort);
});
