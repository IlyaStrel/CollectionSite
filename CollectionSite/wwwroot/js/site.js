function getScanning() {
    var text = $("#ipAddresses").val();

    document.querySelector('.load').style.display = 'block';
    document.querySelector('#ipAddresses').disabled = true;
    document.querySelector('#scanButton').disabled = true;

    $.ajax({
        type: 'GET',
        url: '/consumer/scanning',
        data: { addresses: text },
        success: function (result) {
            document.querySelector('.load').style.display = 'none';
            document.querySelector('#ipAddresses').disabled = false;
            document.querySelector('#scanButton').disabled = false;

            var div = document.querySelector('#result');
            div.innerHTML = '';

            result.forEach(
                host => {
                    var h4 = document.createElement("h4");
                    h4.innerText = 'IP: ' + host.ip;
                    if (host.typeIp != '' && host.typeIp != null)
                        h4.innerText += ' Type: ' + host.typeIp;
                    if (host.name != '' && host.name != null)
                        h4.innerText += ' Host name: ' + host.name;
                    if (host.state != '' && host.state != null)
                        h4.innerText += ' Host state: ' + host.state;
                    div.appendChild(h4);

                    var tablePorts = document.createElement("table"), row;
                    var keys = Object.keys(host.ports[0]);
                    row = tablePorts.insertRow();
                    keys.forEach(
                        key => row.insertCell().innerText = key
                    );

                    host.ports.forEach(
                        port => {
                            row = tablePorts.insertRow();
                            keys.forEach(
                                key => row.insertCell().innerText = port[key]
                            );
                        }
                    );
                    div.appendChild(tablePorts);

                    var tableTrace = document.createElement("table"), row;
                    var keys = Object.keys(host.traces[0]);
                    row = tableTrace.insertRow();
                    keys.forEach(
                        key => row.insertCell().innerText = key
                    );

                    host.traces.forEach(
                        trace => {
                            row = tableTrace.insertRow();
                            keys.forEach(
                                key => row.insertCell().innerText = trace[key]
                            );
                        }
                    );
                    div.appendChild(tableTrace);
                }
            );
        },
        error: function (errorResult) {
            document.querySelector('.load').style.display = 'none';
            document.querySelector('#ipAddresses').disabled = false;
            document.querySelector('#scanButton').disabled = false;
            console.log(errorResult);
        }
    });
};