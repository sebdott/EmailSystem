import React from 'react';
import {Router, Route, Switch} from 'dva/router';
import Dynamic from 'dva/dynamic';

function RouterConfig({history, app}) {
  const EmailIndex = Dynamic({
    app,
    component: () => import('../src/components/Email/EmailIndex'),
  });
  const UserIndex = Dynamic({
    app,
    component: () => import('../src/components/User/UserIndex'),
  });
  const TestControlCenterIndex = Dynamic({
    app,
    component: () =>
      import('../src/components/TestControlCenter/TestControlCenterIndex'),
  });

  return (
    <Router history={history}>
      <Switch>
        <Route exact path="/" component={EmailIndex} />
        <Route exact path="/Email" component={EmailIndex} />
        <Route exact path="/User" component={UserIndex} />
        <Route
          exact
          path="/TestControlCenter"
          component={TestControlCenterIndex}
        />
      </Switch>
    </Router>
  );
}

export default RouterConfig;
