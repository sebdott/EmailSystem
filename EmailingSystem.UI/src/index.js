import '@babel/polyfill';
import dva from 'dva';
import {message} from 'antd';
import {createBrowserHistory as createHistory} from 'history';
import './index.css';
import {browserHistory} from 'dva/router';

const ERROR_MSG_DURATION = 3;

const app = dva({
  history: browserHistory,
  onError(e) {
    const msgs = document.querySelectorAll('.ant-message-notice');
    if (msgs.length < 1) {
      message.error(e, ERROR_MSG_DURATION);
    }
  },
});
app.model(require('./models/email').default);
app.model(require('./models/users').default);
app.model(require('./models/navigation').default);
app.model(require('./models/test').default);
app.router(require('./router').default);

app.start('#root');
