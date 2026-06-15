const http = require('http');

async function testApi() {
    try {
        const loginData = JSON.stringify({ username: 'admin', password: 'password123' });
        
        const loginOptions = {
            hostname: 'localhost',
            port: 5107,
            path: '/auth/login',
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Content-Length': loginData.length
            }
        };

        const token = await new Promise((resolve, reject) => {
            const req = http.request(loginOptions, (res) => {
                let data = '';
                res.on('data', chunk => data += chunk);
                res.on('end', () => resolve(JSON.parse(data).token));
            });
            req.on('error', reject);
            req.write(loginData);
            req.end();
        });

        console.log('Got token:', token ? 'Yes' : 'No');

        // 2. Create FormData payload
        const boundary = '----WebKitFormBoundary7MA4YWxkTrZu0gW';
        let body = '';
        
        const fields = {
            'ComplainantName': 'Test',
            'DefendantName': 'Test',
            'AadhaarNumber': '123456789012',
            'PanNumber': 'ABCDE1234F',
            'AccountNumber': '1234567890',
            'BankName': 'Test Bank',
            'OrderType': '1',
            'FreezeAmount': '0'
        };

        for (const [key, value] of Object.entries(fields)) {
            body += `--${boundary}\r\n`;
            body += `Content-Disposition: form-data; name="${key}"\r\n\r\n`;
            body += `${value}\r\n`;
        }

        const files = ['CourtOrderFile', 'AadhaarCopyFile', 'PanCopyFile'];
        for (const file of files) {
            body += `--${boundary}\r\n`;
            body += `Content-Disposition: form-data; name="${file}"; filename="dummy.png"\r\n`;
            body += `Content-Type: image/png\r\n\r\n`;
            body += `fakeimagecontent\r\n`;
        }

        body += `--${boundary}--\r\n`;

        // 3. Post Case
        const caseOptions = {
            hostname: 'localhost',
            port: 5107,
            path: '/api/court/cases',
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': `multipart/form-data; boundary=${boundary}`,
                'Content-Length': Buffer.byteLength(body)
            }
        };

        const result = await new Promise((resolve, reject) => {
            const req = http.request(caseOptions, (res) => {
                let data = '';
                res.on('data', chunk => data += chunk);
                res.on('end', () => resolve({ status: res.statusCode, data }));
            });
            req.on('error', reject);
            req.write(body);
            req.end();
        });

        console.log('Status Code:', result.status);
        console.log('Response Body:', result.data);

    } catch (e) {
        console.error(e);
    }
}

testApi();
