import React, {Component} from 'react';
import _ from 'lodash';
import {Input, Button, Icon} from 'antd';
import {connect} from 'dva';
import Main from '../MainLayout/Main';
import css from '../../styles/component/TestControlCenter.less';

class TestControlCenterIndex extends Component {
  constructor(props) {
    super(props);
    this.dispatch = this.props.dispatch;

    this.TestButtonList = {
      ResetTestData: {
        Label: 'Reset Test Data',
        Description:
          'This Enables all test data to be resetted (Default/Empty: 50 users)',
        OnClick: this.onResetTestDataClick.bind(this),
      },
      InitiateSetStatusUpdateProcess: {
        Label: 'Initiate Set Status Update Proces',
        Description:
          "This enables initiation of all the user's status update batch run",
        OnClick: this.onInitiateSetStatusUpdateProcessClick.bind(this),
      },
      InitiateSendEmailProcess: {
        Label: 'Initiate Send Email Process',
        Description: 'This enables initiation of send email process batch run',
        OnClick: this.onInitiateSendEmailProcessClick.bind(this),
      },
    };
  }
  componentWillMount() {
    this.dispatch({
      type: 'testModel/initializeState',
    });
    // this.dispatch({
    //   type: 'testModel/getUserList',
    // });
  }
  componentWillUnmount() {
    this.dispatch({
      type: 'testModel/initializeState',
    });
  }
  onInitiateSendEmailProcessClick() {
    this.dispatch({
      type: 'testModel/InitiateSendEmailProcess',
    });
  }
  onInitiateSetStatusUpdateProcessClick() {
    this.dispatch({
      type: 'testModel/InitiateSetStatusUpdateProcess',
    });
  }
  onResetTestDataClick() {
    this.dispatch({
      type: 'testModel/ResetTestData',
    });
  }
  onRecordSizeChange(e) {
    const {value} = e.target;
    this.dispatch({
      type: 'testModel/updateState',
      payload: {
        records: value,
      },
    });
  }
  renderResetInput(currentLabel) {
    const {records} = this.props;
    if (currentLabel === 'ResetTestData') {
      return (
        <div className={css.testControlCenter_description}>
          <Input
            type="number"
            placeholder="Record Size"
            defaultValue="50"
            value={records}
            onChange={this.onRecordSizeChange.bind(this)}
          />
        </div>
      );
    }

    return <div />;
  }

  renderTestButtonList() {
    return _.map(
      this.TestButtonList,
      ({Label, Description, OnClick}, index) => {
        return (
          <div
            key={index + '_testButtonListDiv'}
            className={css.testControlCenter_buttonOuter}>
            <Button
              key={index + '_testButtonList'}
              type="primary"
              onClick={OnClick}>
              {Label}
            </Button>
            <div className={css.testControlCenter_description}>
              {Description}
            </div>
            {this.renderResetInput(index)}
          </div>
        );
      },
    );
  }
  renderScene() {
    return (
      <div className={css.testControlCenter_main}>
        <div>
          {this.renderTestButtonList()}
          <div>
            <Icon type="exclamation-circle" /> Warning: This is a Test Control
            Center, any button you press will cause disruption in the original
            test data, use it with care.
          </div>
        </div>
      </div>
    );
  }
  render() {
    const {awaitingResponse} = this.props;
    return (
      <Main content={this.renderScene()} awaitingResponse={awaitingResponse} />
    );
  }
}

const mapStatesToProps = ({testModel}) => {
  return {
    ...testModel,
  };
};

export default connect(mapStatesToProps)(TestControlCenterIndex);
