import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditRecordComponent } from './create-edit-record.component';

describe('CreateEditRecordComponent', () => {
  let component: CreateEditRecordComponent;
  let fixture: ComponentFixture<CreateEditRecordComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditRecordComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditRecordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
